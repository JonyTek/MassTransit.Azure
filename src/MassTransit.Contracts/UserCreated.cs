using System;
using System.Collections.Generic;
using System.Linq;

namespace MassTransit.Contracts
{
    public class AccountId : AbstractIdentity
    {
        public static AccountId New => new AccountId {Value = Guid.NewGuid()};

        private AccountId()
        {
        }
    }

    public class RegisterAccount
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class AccountRegistered : AbstractDomainEvent<AccountId>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public AccountRegistered(Account account) 
            : base(account)
        {
        }
    }

    public class AccountName
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }

        private AccountName()
        {
        }

        public static AccountName Create(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentNullException(nameof(firstName));
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentNullException(nameof(lastName));
            }

            return new AccountName
            {
                FirstName = firstName, LastName = lastName
            };
        }
    }

    public class Account : AggregateRoot<AccountId>
    {
        public AccountName AccountName { get; private set; }

        private Account()
        { 
        }

        public static Account Register(RegisterAccount command)
        {
            var account = new Account
            {
                Id = AccountId.New,
                AccountName = AccountName.Create(command.FirstName, command.LastName)
            };

            account.Raise(new AccountRegistered(account)
            {
                FirstName = account.AccountName.FirstName,
                LastName = account.AccountName.LastName
            });

            return account;
        }

        public override void Apply(AbstractDomainEvent<AccountId> domainEvent)
        {
            switch (domainEvent)
            {
                case AccountRegistered x:
                    AccountName = AccountName.Create(x.FirstName, x.LastName);
                    break;
                default:
                    throw new Exception($"No apply function registered for type {domainEvent.GetType().Name}");
            }
        }
    }

    public abstract class AggregateRoot<TIdentity> where TIdentity : AbstractIdentity
    {
        public long Version { get; private set; }
        public TIdentity Id { get; protected set; }

        private readonly ISet<Guid> _inbox = new HashSet<Guid>();

        private readonly ISet<AbstractDomainEvent<TIdentity>> _outbox =
            new HashSet<AbstractDomainEvent<TIdentity>>(AbstractDomainEventComparer<TIdentity>.Instance);

        public IReadOnlyCollection<AbstractDomainEvent<TIdentity>> Outbox => _outbox.ToArray();

        protected void Raise(AbstractDomainEvent<TIdentity> domainEvent)
        {
            Apply(domainEvent);

            _outbox.Add(domainEvent);

            Version++;
        }

        public void Handle<TId>(AbstractDomainEvent<TId> domainEvent, Action<AbstractDomainEvent<TId>> action)
            where TId : AbstractIdentity
        {
            if (_inbox.Contains(domainEvent.Id))
            {
                return;
            }

            action(domainEvent);

            _inbox.Add(domainEvent.Id);
        }

        public abstract void Apply(AbstractDomainEvent<TIdentity> domainEvent);
    }

    public abstract class AbstractDomainEvent<TIdentity>  where TIdentity : AbstractIdentity
    {
        public Guid Id { get; private set; }
        public TIdentity AggregateId { get; private set; }
        public long AggregateVersion { get; private set; }

        protected AbstractDomainEvent(AggregateRoot<TIdentity> aggregate)
        {
            Id = Guid.NewGuid();
            AggregateId = aggregate.Id;
            AggregateVersion = aggregate.Version;
        }
    }

    public class AbstractDomainEventComparer<TIdentity> : IEqualityComparer<AbstractDomainEvent<TIdentity>>
        where TIdentity : AbstractIdentity
    {
        public static IEqualityComparer<AbstractDomainEvent<TIdentity>> Instance =>
            new AbstractDomainEventComparer<TIdentity>();

        public bool Equals(AbstractDomainEvent<TIdentity> x, AbstractDomainEvent<TIdentity> y) =>
            x?.AggregateId == y?.AggregateId;

        public int GetHashCode(AbstractDomainEvent<TIdentity> obj) => obj.AggregateId.GetHashCode();
    }

    public abstract class AbstractIdentity
    {
        public Guid Value { get; set; }
    }
}