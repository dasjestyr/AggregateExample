using System;

namespace AggregateExample2.Domain
{
    public class WalletNotFoundException : Exception
    {
        public WalletNotFoundException(string message)
            : base(message)
        {
        }
    }
}