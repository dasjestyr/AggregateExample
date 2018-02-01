using System;
using System.Collections.Generic;
using System.Linq;
using AggregateExample2.Domain.Customer.Events;
using AggregateExample2.Infrastructure;

namespace AggregateExample2.Domain.Customer
{
    public class Customer : AggregateRoot
    {
        private readonly List<Wallet> _wallets = new List<Wallet>();

        public AccountNumber AccountNumber { get; private set; }

        public string Name { get; private set; }

        private Customer()
        {
            RegisterEventRoute<SignedUp>(Apply);
            RegisterEventRoute<WalletCreated>(Apply);
            RegisterEventRoute<DepositedMoney>(Apply);
            RegisterEventRoute<WithdrewMoney>(Apply);
        }

        public static Customer Create(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            
            var id = Guid.NewGuid();

            var customer = new Customer
            {
                Id = id,
                Name = name
            };

            customer.RaiseEvent<SignedUp>(e =>
            {
                e.Id = id;
                e.Name = name;
            });

            return customer;
        }

        public void CreateWallet(Money initialBalance)
        {
            if(initialBalance < Money.Default)
                throw new NegativeOpeningBalanceException("Cannot create a new wallet with a negative balance.");

            var createdWallet = Wallet.Create(new AccountNumber(Id), initialBalance);
            RaiseEvent(createdWallet);
        }

        public void DespositMoney(WalletId walletId, Money amount)
        {
            var wallet = _wallets.SingleOrDefault(w => w.WalletId == walletId);
            if(wallet == null)
                throw new WalletNotFoundException($"Wallet {walletId} was not found.");

            var e = wallet.Deposit(amount);
            RaiseEvent(e);
        }

        public void WithdrawMoney(WalletId walletId, Money amount)
        {
            var wallet = _wallets.SingleOrDefault(w => w.WalletId == walletId);
            if(wallet == null)
                throw new WalletNotFoundException($"Wallet {walletId} was not found.");

            if(amount > wallet.Balance)
                throw new InsufficientFundsException($"Insufficient funds to withdraw {amount}.");

            var e = wallet.Withdraw(amount);
            RaiseEvent(e);
        }

        public void TransferToWallet(WalletId sourceId, WalletId destinationId, Money amount)
        {
            var source = _wallets.SingleOrDefault(w => w.WalletId == sourceId);
            var destination = _wallets.SingleOrDefault(w => w.WalletId == destinationId);
            if(source == null || destination == null)
                throw new WalletNotFoundException("Could not find one of the specified wallets.");

            if(source.Balance < amount)
                throw new InsufficientFundsException("The source wallet does not have enough funds to transfer to the destination.");

            // This *could* just call WithdrawnMoney and DepositMoney, but I want to separate them
            // in order to create an atomic transaction. If it errors before raising the events,
            // then the events will not persist.
            var withdrawn = source.Withdraw(amount);
            var deposited = destination.Deposit(amount);
            
            RaiseEvent(withdrawn);
            RaiseEvent(deposited);
        }

        private void Apply(SignedUp e)
        {
            Id = e.Id;
            AccountNumber = new AccountNumber(Id);
            Name = e.Name;
        }

        private void Apply(WalletCreated e)
        {
            var wallet = new Wallet();
            wallet.Apply(e);
            _wallets.Add(wallet);
        }

        private void Apply(DepositedMoney e)
        {
            var walletId = new WalletId(e.EntityId);
            var wallet = _wallets.Single(w => w.WalletId == walletId);
            wallet.Apply(e);
        }

        private void Apply(WithdrewMoney e)
        {
            var walletId = new WalletId(e.EntityId);
            var wallet = _wallets.Single(w => w.WalletId == walletId);
            wallet.Apply(e);
        }
    }
}
