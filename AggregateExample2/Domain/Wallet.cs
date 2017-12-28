using System;
using System.Diagnostics;
using AggregateExample2.Domain.Customer;
using AggregateExample2.Domain.Customer.Events;
using AggregateExample2.Infrastructure;

namespace AggregateExample2.Domain
{
    internal class Wallet : Entity
    {
        public WalletId WalletId { get; private set; }

        public Money Balance { get; private set; }

        public static WalletCreated Create(AccountNumber accountNumber, Money initalBalance)
        {
            if(accountNumber == null)
                throw new ArgumentNullException(nameof(accountNumber));

            if(initalBalance < Money.Default)
                throw new NegativeOpeningBalanceException("Cannot open a wallet with a negative balance.");

            /* this is just to facilitate the demo */
            var newID = Guid.NewGuid();
            Program.TemporaryWalletValue.LastWalletId = new WalletId(newID);
            /****************************************/

            return new WalletCreated
            {
                EntityId = newID,
                InitialBalance = initalBalance.Value
            };
        }

        public DepositedMoney Deposit(Money amount)
        {
            if (amount.Value < 0)
                throw new InvalidOperationException("Cannot deposit negative amount.");

            return new DepositedMoney
            {
                EntityId = Id,
                Amount = amount.Value
            };
        }

        public WithdrewMoney Withdraw(Money amount)
        {
            if(amount.Value < 0)
                throw new InvalidOperationException("Cannot withdrawn a negative amount.");

            return new WithdrewMoney
            {
                EntityId = Id,
                Amount = amount.Value
            };
        }

        internal void Apply(WalletCreated e)
        {
            Id = e.EntityId;
            WalletId = new WalletId(Id);
            Balance = new Money(e.InitialBalance);
            Console.WriteLine($"Wallet {Id} balance is now {Balance}");
        }

        internal void Apply(DepositedMoney e)
        {
            var amount = new Money(e.Amount);
            Balance = Balance.Add(amount);
            Console.WriteLine($"Wallet {Id} balance is now {Balance}");
        }

        internal void Apply(WithdrewMoney e)
        {
            var amount = new Money(e.Amount);
            Balance = Balance.Subtract(amount);
            Console.WriteLine($"Wallet {Id} balance is now {Balance}");
        }
    }
}