using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using MassTransit.Contracts;

namespace MassTransit.Azure.Consumer
{
    public static class InMemoryRepository<T, TId> where T : AggregateRoot<TId> where TId : AbstractIdentity
    {
        private static readonly ConcurrentDictionary<Guid, T> Database = new ConcurrentDictionary<Guid, T>();

        public static Task Insert(T aggregate)
        {
            Database[aggregate.Id.Value] = aggregate;

            return Task.CompletedTask;
        }

        public static Task<T> Retrieve(Guid id)
        {
            var result = Database.ContainsKey(id) ? Database[id] : default;

            return Task.FromResult(result);
        }
    }
}