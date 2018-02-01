using System;

namespace AggregateExample3
{
    public class InvalidTransactionAmountException : Exception
    {
        public InvalidTransactionAmountException(string message)
            : base(message) { }
    }
}