using System;
using System.Threading.Tasks;
using MassTransit.Contracts;

namespace MassTransit.Azure.Consumer.Handlers
{
    public class DoThingHandler : IHandler<DoThing>
    {
        public async Task Consume(ConsumeContext<DoThing> context)
        {
            Console.WriteLine($"Doing thing - {context.Message.Id}");

            await context.SchedulePublish(DateTime.Now.AddSeconds(10), new DoSomethingElse {Id = context.Message.Id});
        }

        public Task Consume(ConsumeContext<Fault<DoThing>> context)
        {
            Console.WriteLine($"Doing thing went wrong - {context.Message.Message.Id}");
            
            return Task.CompletedTask;
        }
    }
}