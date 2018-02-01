using System;

namespace AggregateExample3
{
    public class Transaction : ValueObject
    {
        public TransactionType Type { get; }

        public DateTimeOffset TransactionDate { get; }

        public Money Amount { get; }

        public Transaction(TransactionType type, Money amount, DateTimeOffset transactionDate)
        {
            if(amount < Money.Default)
                throw new ArgumentException("Transactions must be conducted using money values greater than 0.");

            Type = type;
            TransactionDate = transactionDate;
            Amount = amount;
        }   
    }
}