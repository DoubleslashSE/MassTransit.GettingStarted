using Company.Consumers;
using GettingStarted.ClaimCheck;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace GettingStarted
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        // Example of setting up MassTransit in the host builder:
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    //ClaimCheckStore
                    var store = new InMemoryClaimCheckStore();
                    const int threshold = 256 * 1024;

                    var factory = new ClaimCheckSerializerFactory(store, threshold);

                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<GettingStartedConsumer>();

                        x.UsingInMemory((ctx, cfg) =>
                        {
                            cfg.ClearSerialization();
                            cfg.AddSerializer(factory);
                            cfg.AddDeserializer(factory);

                            cfg.ReceiveEndpoint("my_queue", e =>
                            {
                                e.ConfigureConsumer<GettingStartedConsumer>(ctx);
                            });
                        });
                    });

                    services.AddHostedService<Worker>();
                });
    }
}
