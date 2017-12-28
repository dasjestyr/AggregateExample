using System;

namespace AggregateExample2.Domain
{
    public class InsufficientFundsException : Exception
    {
        public InsufficientFundsException(string message)
            : base(message)
        {
        }
    }
}