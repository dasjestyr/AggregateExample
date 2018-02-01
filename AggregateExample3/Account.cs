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
            
            var accountDebited = _ledger.Debit(amount, transactionDate);
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
            var e = _ledger.Credit(amount, transactionDate);
            RaiseEvent(e);
            TryResumeAccount();
        }

        /// <summary>
        /// This is what I was talking about when I said the method on the AR simply mediates
        /// between two instances of the same aggregate (through the destination Account root)
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="destination"></param>
        /// <param name="transactionDate"></param>
        public void Transfer(Money amount, Account destination, DateTimeOffset transactionDate)
        {
            EnsureAccountCanWithdraw();

            // events are already handled on their respective root
            Debit(amount, transactionDate);
            destination.Credit(amount, transactionDate);
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
    }
}
