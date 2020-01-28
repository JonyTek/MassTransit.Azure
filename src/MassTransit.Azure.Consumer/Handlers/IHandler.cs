using System.Threading.Tasks;

namespace MassTransit.Azure.Consumer.Handlers
{
    public interface IHandler<in T> : IConsumer<T>, IConsumer<Fault<T>>
        where T : class 
    {
    }
}