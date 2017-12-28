using System;
using AggregateExample2.Infrastructure;

namespace AggregateExample2.Domain.Customer
{
    public class WalletId : ValueObject<WalletId>
    {
        public Guid Value { get; }

        public WalletId(Guid value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}