using GettingStarted.ClaimCheck.Interfaces;
using MassTransit;
using MassTransit.Serialization;
using System.Net.Mime;

namespace GettingStarted.ClaimCheck;

public class ClaimCheckSerializerFactory : ISerializerFactory
{
    private readonly IClaimCheckStore _store;
    private readonly int _thresholdBytes;

    public ClaimCheckSerializerFactory(IClaimCheckStore store, int thresholdBytes)
    {
        _store = store;
        _thresholdBytes = thresholdBytes;
    }

    public ContentType ContentType => SystemTextJsonMessageSerializer.Instance.ContentType;

    public IMessageSerializer CreateSerializer()
    {
        return new ClaimCheckMessageSerializer(_store, _thresholdBytes);
    }

    public IMessageDeserializer CreateDeserializer()
    {
        return new ClaimCheckMessageDeserializer(_store);
    }
}