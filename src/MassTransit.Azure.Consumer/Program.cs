using System;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit.Azure.Consumer.Handlers;
using MassTransit.Azure.ServiceBus.Core;
using Microsoft.Extensions.DependencyInjection;

namespace MassTransit.Azure.Consumer
{
    class Program
    {
        private static string connectionString =
            "Endpoint=sb://jony-test.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=6DPF4of1TNasG+0OEFkH0W9H6Sb8IqAJzrTdhNs0s4c=";

        static async Task Main(string[] args)
        {
            var bus = SetupBus();

            await RunApplication(bus);
        }

        private static IBusControl SetupBus()
        {
            var services = new ServiceCollection();

            services.AddMassTransit(x =>
            {
                x.AddConsumersFromNamespaceContaining<DoThingHandler>();

                x.AddBus(provider => Bus.Factory.CreateUsingAzureServiceBus(serviceBus =>
                {
                    serviceBus.Host(connectionString);

                    serviceBus.ReceiveEndpoint("consumer", endpoint =>
                    {
                        endpoint.ConfigureConsumers(provider);

                        endpoint.UseMessageRetry(retry => retry.Immediate(5));
                    });
                }));
            });

            return services.BuildServiceProvider().GetService<IBusControl>();
        } 

        private static async Task RunApplication(IBusControl busControl)
        {
            await busControl.StartAsync();

            do
            {
                Console.WriteLine("Waiting....");

                if (Console.ReadKey().Key == ConsoleKey.Escape)
                {
                    break;
                }

            } while (true);

            await busControl.StopAsync();
        }
    }
}
