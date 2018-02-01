using System;
using System.Collections.Generic;

namespace AggregateExample3
{
    public class Ledger : Entity
    {
        private readonly List<Transaction> _transactions = new List<Transaction>();

        public Money Balance => CalculateBalance();

        public AccountDebited Debit(Transaction transaction)
        {
            transaction.SetType(TransactionType.Debit);
            _transactions.Add(transaction);

            return new AccountDebited
            {
                Amount = transaction.Amount.Amount,
                TransactionDate = transaction.TransactionDate
            };
        }

        public AccountCredited Credit(Transaction transaction)
        {
            transaction.SetType(TransactionType.Credit);
            _transactions.Add(transaction);

            return new AccountCredited
            {
                Amount = transaction.Amount.Amount,
                TransactionDate = transaction.TransactionDate
            };
        }

        public MoneyTransferredOut TransferOut(Transaction transaction, Guid destinationAccountId)
        {
            transaction.SetType(TransactionType.TransferOut);
            _transactions.Add(transaction);

            return new MoneyTransferredOut
            {
                Amount = transaction.Amount.Amount,
                DestinationAccountId = destinationAccountId,
                TransferDate = transaction.TransactionDate
            };
        }

        public MoneyTransferredIn TransferIn(Transaction transaction, Guid sourceAccountId)
        {
            transaction.SetType(TransactionType.TransferIn);
            _transactions.Add(transaction);
            return new MoneyTransferredIn
            {
                Amount = transaction.Amount.Amount,
                SourceAccountId = sourceAccountId,
                TransferDate = transaction.TransactionDate
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
                    case TransactionType.TransferOut:
                        balance -= transaction.Amount;
                        break;
                    case TransactionType.Credit:
                    case TransactionType.TransferIn:
                        balance += transaction.Amount;
                        break;
                }
            }

            return balance;
        }

        // In a full event-sourced solution, I'd usually put the event applicators
        // down here and those would be what *actually* update the state. See
        // AggregateExample2 for a demonstration of that
    }
}