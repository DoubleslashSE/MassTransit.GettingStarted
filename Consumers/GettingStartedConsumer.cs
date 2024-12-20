using Microsoft.Extensions.Logging;

namespace Company.Consumers
{
    using Contracts;
    using MassTransit;
    using System.Threading.Tasks;

    public class GettingStartedConsumer :
        IConsumer<GettingStarted>
    {
        readonly ILogger<GettingStartedConsumer> _logger;

        public GettingStartedConsumer(ILogger<GettingStartedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<GettingStarted> context)
        {
            _logger.LogInformation("Received Text: {Text}", context.Message.Value);
            return Task.CompletedTask;
        }
    }
}