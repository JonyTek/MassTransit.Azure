using System;

namespace MassTransit.Contracts.Specs
{
    public class Given
    {
        public void And(string given, Action action)
        {
            action();
        }
    }
}