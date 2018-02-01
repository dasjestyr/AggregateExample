namespace AggregateExample3
{
    public class Money : ValueObject
    {
        public static Money Default = new Money(0);

        public decimal Amount { get; }

        public Money(decimal amount)
        {
            Amount = amount;
        }

        public static Money operator +(Money left, Money right)
        {
            return new Money(left.Amount + right.Amount);
        }

        public static Money operator -(Money left, Money right)
        {
            return new Money(left.Amount - right.Amount);
        }

        public static bool operator <(Money left, Money right)
        {
            return left.Amount < right.Amount;
        }

        public static bool operator >(Money left, Money right)
        {
            return left.Amount > right.Amount;
        }

        public static bool operator <=(Money left, Money right)
        {
            return left.Amount <= right.Amount;
        }

        public static bool operator >=(Money left, Money right)
        {
            return left.Amount >= right.Amount;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Money);
        }

        protected bool Equals(Money other)
        {
            return Amount == other?.Amount;
        }

        public override int GetHashCode()
        {
            return Amount.GetHashCode();
        }
    }
}