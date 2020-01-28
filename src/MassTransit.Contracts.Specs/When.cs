using System;

namespace MassTransit.Contracts.Specs
{
    public class When
    {
        public void And(string when, Action action)
        {
            action();
        }
    }
}