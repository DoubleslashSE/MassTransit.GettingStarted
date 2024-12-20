using GettingStarted.ClaimCheck.Interfaces;
using MassTransit;
using MassTransit.Serialization;
using System.Net.Mime;
using System.Text.Json;

namespace GettingStarted.ClaimCheck;

public class ClaimCheckMessageSerializer : IMessageSerializer
{
    private readonly IClaimCheckStore _store;
    private readonly int _thresholdBytes;
    private readonly IMessageSerializer _innerSerializer;

    public ClaimCheckMessageSerializer(IClaimCheckStore store, int thresholdBytes)
    {
        _store = store;
        _thresholdBytes = thresholdBytes;
        _innerSerializer = SystemTextJsonMessageSerializer.Instance;
    }

    public ContentType ContentType => _innerSerializer.ContentType;

    public MessageBody GetMessageBody<T>(SendContext<T> context) where T : class
    {
        var originalBody = _innerSerializer.GetMessageBody(context);
        var originalBytes = originalBody.GetBytes();

        if (originalBytes.Length <= _thresholdBytes) return originalBody;

        var reference = _store.StoreAsync(originalBytes).GetAwaiter().GetResult();

        var claimCheckPayload = new
        {
            __claim_check__ = true,
            reference
        };

        var claimCheckJson = JsonSerializer.SerializeToUtf8Bytes(claimCheckPayload);
        return new BytesMessageBody(claimCheckJson);

    }

    public void Probe(ProbeContext context)
    {
        var scope = context.CreateScope("claimCheckSerializer");
        scope.Add("thresholdBytes", _thresholdBytes);
    }
}