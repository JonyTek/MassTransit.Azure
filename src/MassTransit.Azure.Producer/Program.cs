using System;
using System.Threading.Tasks;
using MassTransit.Azure.ServiceBus.Core;
using MassTransit.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace MassTransit.Azure.Producer
{
    class Program
    {
        private static string connectionString = "";

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
                x.AddBus(provider => Bus.Factory.CreateUsingAzureServiceBus(serviceBus =>
                {
                    serviceBus.Host(connectionString);
                }));
            });

            return services.BuildServiceProvider().GetService<IBusControl>();
        } 

        private static async Task RunApplication(IBusControl busControl) 
        {
            do
            {
                Console.WriteLine("Sending message....");

                await busControl.Publish(new DoThing {Id = Guid.NewGuid().ToString()});

                Console.WriteLine("Press any key to send message....");

                if (Console.ReadKey().Key == ConsoleKey.Escape)
                {
                    break;
                }

            } while (true);
        }
    }
}
