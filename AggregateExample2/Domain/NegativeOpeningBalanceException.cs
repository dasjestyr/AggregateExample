using System;

namespace AggregateExample2.Domain
{
    public class NegativeOpeningBalanceException : Exception
    {
        public NegativeOpeningBalanceException(string message)
            : base(message)
        {
        }
    }
}