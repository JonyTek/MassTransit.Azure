using System;

namespace MassTransit.Contracts.Specs
{
    public class Specification<TSystemUnderTest>
    {
        protected TSystemUnderTest SystemUnderTest { get; set; }

        public Given Given(string given, Action action)
        {
            action();

            return new Given();
        }

        public When When(string when, Action action)
        {
            action();

            return new When();
        }
    }
}