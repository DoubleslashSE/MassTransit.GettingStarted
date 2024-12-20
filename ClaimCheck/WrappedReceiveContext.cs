using MassTransit;
using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

namespace GettingStarted.ClaimCheck;

public class WrappedReceiveContext : ReceiveContext
{
    private readonly ReceiveContext _original;

    public WrappedReceiveContext(ReceiveContext original, MessageBody newBody)
    {
        _original = original;
        Body = newBody;
    }

    public TimeSpan ElapsedTime => _original.ElapsedTime;
    public Uri InputAddress => _original.InputAddress;
    public ContentType ContentType => _original.ContentType;
    public bool Redelivered => _original.Redelivered;
    public Headers TransportHeaders => _original.TransportHeaders;
    public Task ReceiveCompleted => _original.ReceiveCompleted;
    public bool IsDelivered => _original.IsDelivered;
    public bool IsFaulted => _original.IsFaulted;
    public ISendEndpointProvider SendEndpointProvider => _original.SendEndpointProvider;
    public IPublishEndpointProvider PublishEndpointProvider => _original.PublishEndpointProvider;
    public bool PublishFaults => _original.PublishFaults;
    public MessageBody Body { get; }

    public CancellationToken CancellationToken => _original.CancellationToken;

    public Task NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType) where T : class =>
        _original.NotifyConsumed(context, duration, consumerType);

    public Task NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception) where T : class =>
        _original.NotifyFaulted(context, duration, consumerType, exception);

    public Task NotifyFaulted(Exception exception) =>
        _original.NotifyFaulted(exception);

    public void AddReceiveTask(Task task) =>
        _original.AddReceiveTask(task);

    public bool HasPayloadType(Type payloadType) => _original.HasPayloadType(payloadType);

    public bool TryGetPayload<T>(out T payload) where T : class =>
        _original.TryGetPayload(out payload);

    public T GetOrAddPayload<T>(PayloadFactory<T> payloadFactory) where T : class =>
        _original.GetOrAddPayload(payloadFactory);

    public T AddOrUpdatePayload<T>(PayloadFactory<T> addFactory, UpdatePayloadFactory<T> updateFactory) where T : class =>
        _original.AddOrUpdatePayload(addFactory, updateFactory);
}