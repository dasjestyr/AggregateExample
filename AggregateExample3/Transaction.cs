using System;

namespace AggregateExample3
{
    public class Transaction : Entity
    {
        public TransactionType Type { get; private set; }

        public DateTimeOffset TransactionDate { get; }

        public Money Amount { get; }

        public Transaction(Money amount, DateTimeOffset transactionDate)
            : this(Guid.NewGuid(), amount, transactionDate)
        {
        }

        public Transaction(Guid id, Money amount, DateTimeOffset transactionDate)
        {
            if (amount < Money.Default)
                throw new ArgumentException("Transactions must be conducted using money values greater than 0.");

            Id = id;
            TransactionDate = transactionDate;
            Amount = amount;
        }

        public void SetType(TransactionType type)
        {
            Type = type;
        }
    }
}