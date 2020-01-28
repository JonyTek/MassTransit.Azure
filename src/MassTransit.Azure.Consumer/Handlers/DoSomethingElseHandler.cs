using System;
using System.Threading.Tasks;
using MassTransit.Contracts;

namespace MassTransit.Azure.Consumer.Handlers
{
    public class DoSomethingElseHandler : IHandler<DoSomethingElse>
    {
        public Task Consume(ConsumeContext<DoSomethingElse> context)
        {
            Console.WriteLine($"Doing something else - {context.Message.Id}");

            return Task.CompletedTask; 
        }

        public Task Consume(ConsumeContext<Fault<DoSomethingElse>> context)
        {
            Console.WriteLine($"Doing something else has gone wrong- {context.Message.Message.Id}");

            return Task.CompletedTask;
        }
    }
}