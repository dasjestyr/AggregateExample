using System;
using AggregateExample2.Infrastructure;

namespace AggregateExample2.Domain
{
    public class AccountNumber : ValueObject<AccountNumber>
    {
        public Guid Value { get; }

        public AccountNumber(Guid value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}