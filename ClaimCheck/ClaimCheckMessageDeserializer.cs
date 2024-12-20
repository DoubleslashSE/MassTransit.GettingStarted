#nullable enable
using GettingStarted.ClaimCheck.Interfaces;
using MassTransit;
using MassTransit.Serialization;
using System;
using System.Net.Mime;
using System.Text.Json.Nodes;

namespace GettingStarted.ClaimCheck;

public class ClaimCheckMessageDeserializer : IMessageDeserializer
{
    private readonly IClaimCheckStore _store;
    private readonly IMessageDeserializer _innerDeserializer;

    public ClaimCheckMessageDeserializer(IClaimCheckStore store)
    {
        _store = store;
        _innerDeserializer = SystemTextJsonMessageSerializer.Instance;
    }

    public ContentType ContentType => _innerDeserializer.ContentType;

    public void Probe(ProbeContext context)
    {
        context.CreateScope("claimCheckDeserializer");
    }

    public ConsumeContext Deserialize(ReceiveContext receiveContext)
    {
        var bodyBytes = receiveContext.GetBody();
        if (!IsClaimCheck(bodyBytes, out var reference)) return _innerDeserializer.Deserialize(receiveContext);

        var originalBytes = _store.RetrieveAsync(reference).GetAwaiter().GetResult();
        var originalBody = new BytesMessageBody(originalBytes);

        var wrappedContext = new WrappedReceiveContext(receiveContext, originalBody);

        return _innerDeserializer.Deserialize(wrappedContext);
    }

    public SerializerContext Deserialize(MessageBody messageBody, Headers headers, Uri? destinationAddress)
    {
        var bodyBytes = messageBody.GetBytes();
        if (!IsClaimCheck(bodyBytes, out var reference))
            return _innerDeserializer.Deserialize(messageBody, headers, destinationAddress);

        var originalBytes = _store.RetrieveAsync(reference).GetAwaiter().GetResult();
        var originalBody = new BytesMessageBody(originalBytes);
        return _innerDeserializer.Deserialize(originalBody, headers, destinationAddress);
    }

    public MessageBody GetMessageBody(string text)
    {
        return _innerDeserializer.GetMessageBody(text);
    }

    private static bool IsClaimCheck(byte[] bodyBytes, out string reference)
    {
        reference = string.Empty;
        try
        {
            var node = JsonNode.Parse(bodyBytes);
            if (node is JsonObject obj &&
                obj.TryGetPropertyValue("__claim_check__", out var claimNode) &&
                claimNode!.GetValue<bool>())
            {
                var refNode = obj["reference"]?.GetValue<string>();
                if (!string.IsNullOrWhiteSpace(refNode))
                {
                    reference = refNode;
                    return true;
                }
            }
        }
        catch
        {
            // Not a claim-check
        }

        return false;
    }
}