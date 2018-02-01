using System;
using System.Collections.Generic;

namespace AggregateExample3
{
    public class Ledger : Entity
    {
        private readonly List<Transaction> _transactions = new List<Transaction>();

        public Money Balance => CalculateBalance();

        public AccountDebited Debit(Money amount, DateTimeOffset transactionDate)
        {
            var transaction = new Transaction(TransactionType.Debit, amount, transactionDate);
            _transactions.Add(transaction);

            return new AccountDebited
            {
                Amount = transaction.Amount.Amount,
                TransactionDate = transaction.TransactionDate
            };
        }

        public AccountCredited Credit(Money amount, DateTimeOffset transactionDate)
        {
            var transaction = new Transaction(TransactionType.Credit, amount, transactionDate);
            _transactions.Add(transaction);

            return new AccountCredited
            {
                Amount = transaction.Amount.Amount,
                TransactionDate = transaction.TransactionDate
            };
        }

        private Money CalculateBalance()
        {
            var balance = Money.Default;
            foreach (var transaction in _transactions)
            {
                switch (transaction.Type)
                {
                    case TransactionType.Debit:
                        balance -= transaction.Amount;
                        break;
                    case TransactionType.Credit:
                        balance += transaction.Amount;
                        break;
                }
            }

            return balance;
        }
    }
}