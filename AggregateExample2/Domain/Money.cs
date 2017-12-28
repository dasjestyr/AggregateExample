using AggregateExample2.Infrastructure;

namespace AggregateExample2.Domain
{
    public class Money : ValueObject<Money>
    {
        public static Money Default = new Money(0M);

        public decimal Value { get; }

        public Money(decimal value)
        {
            Value = value;
        }

        public Money Add(Money amount)
        {
            var newAmount = Value + amount.Value;
            return new Money(newAmount);
        }

        public Money Subtract(Money amount)
        {
            var newAmount = Value - amount.Value;
            return new Money(newAmount);
        }

        public static bool operator >(Money left, Money right)
        {
            return left.Value > right.Value;
        }

        public static bool operator <(Money left, Money right)
        {
            return left.Value < right.Value;
        }

        public override string ToString()
        {
            return Value.ToString("C");
        }
    }
}