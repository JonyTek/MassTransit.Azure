using System.Linq;
using Shouldly;

namespace MassTransit.Contracts.Specs
{
    public class GivenAnAccountIsRegistered : Specification<Account>
    {
        private RegisterAccount _command;

        public GivenAnAccountIsRegistered()
        {
            Given($"a valid {nameof(RegisterAccount)} command",
                () => _command = new RegisterAccount {FirstName = "Jony", LastName = "Swieboda"});

            When($"the command is handled by the account", () => SystemUnderTest = Account.Register(_command));
        }

        [Then]
        public void TheFirstNameShouldBe() => SystemUnderTest.AccountName.FirstName.ShouldBe(_command.FirstName);

        [Then]
        public void TheLastNameShouldBe() => SystemUnderTest.AccountName.LastName.ShouldBe(_command.LastName);

        [Then]
        public void TheOutboxShouldContainOneDomainEvent() => SystemUnderTest.Outbox.Count.ShouldBe(1);

        [Then]
        public void TheSingleOutboxDomainEventShouldBeOfType() =>
            SystemUnderTest.Outbox.First().ShouldBeOfType<AccountRegistered>();
    }
}
