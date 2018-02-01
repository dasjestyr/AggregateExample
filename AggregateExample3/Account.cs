using System;

namespace AggregateExample3
{
    /// <summary>
    /// Account is the root of the Account-Ledger aggregate.
    /// </summary>
    public class Account : AggregateRoot
    {
        private readonly Money _overdraftLimit = new Money(200);
        private readonly Ledger _ledger = new Ledger();
        private bool _accountIsSuspended;

        public Money Balance => _ledger.Balance;

        private Account() { }

        public static Account Create()
        {
            return Create(Guid.NewGuid());
        }

        public static Account Create(Guid id)
        {
            var account = new Account {Id = id};
            var created = new AccountCreated();
            account.RaiseEvent(created);

            return account;
        }

        public void Debit(Money amount, DateTimeOffset transactionDate)
        {
            EnsureAccountCanWithdraw();

            var transaction = new Transaction(amount, transactionDate);
            var accountDebited = _ledger.Debit(transaction);
            RaiseEvent(accountDebited);

            if (Balance > _overdraftLimit)
                return;

            // if the account crosses the limit, allow it but then suspend it until zero balance
            // a real world scneario might be more complex. For example, the maximum negative 
            // balance would be separate than how much of an overdraft is permitted in a single
            // transaction.

            var lockedEvent = new AccountSuspended {SuspendedDate = DateTimeOffset.UtcNow};
            _accountIsSuspended = true;
            RaiseEvent(lockedEvent);
        }

        public void Credit(Money amount, DateTimeOffset transactionDate)
        {
            var transaction = new Transaction(amount, transactionDate);
            var e = _ledger.Credit(transaction);
            RaiseEvent(e);
            TryResumeAccount();
        }

        public void TransferIn(Money amount, Account source, DateTimeOffset transactionDate)
        {
            var transaction = new Transaction(amount, transactionDate);
            var e = _ledger.TransferIn(transaction, source.Id);
            RaiseEvent(e);
        }
        
        /// <summary>
        /// This is what I was talking about when I said the method on the AR simply mediates
        /// between two instances of the same aggregate (through the destination Account root)
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="destination"></param>
        /// <param name="transactionDate"></param>
        public void TransferOut(Money amount, Account destination, DateTimeOffset transactionDate)
        {
            EnsureAccountCanWithdraw();
            
            var transaction = new Transaction(amount, transactionDate);
            var e = _ledger.TransferOut(transaction, destination.Id);
            destination.TransferIn(amount, this, transactionDate);
            RaiseEvent(e);
        }

        private void EnsureAccountCanWithdraw()
        {
            if (_accountIsSuspended)
                throw new AccountSuspendedException();
        }

        private void TryResumeAccount()
        {
            if (!_accountIsSuspended || Balance < Money.Default)
                return;

            _accountIsSuspended = false;
            var resumed = new AccountResumed { ResumedDate = DateTimeOffset.UtcNow };
            RaiseEvent(resumed);
        }

        // In a full event-sourced solution, I'd usually put the event applicators
        // down here and those would be what *actually* update the state. If the event
        // belongs to a subordinate entity, I would use the apply event in the root to
        // forward that event to the correct entity. See AggregateExample2 for a 
        // demonstration of that.
    }
}
