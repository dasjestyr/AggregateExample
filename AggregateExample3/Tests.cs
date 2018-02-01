using System;
using System.Linq;
using Xunit;

namespace AggregateExample3
{
    public class Tests
    {
        private readonly Guid _accountAid = Guid.Parse("4e1616cf-7306-490b-8873-7296c0b6ee9a");
        private readonly Guid _accountBid = Guid.Parse("aa30e43b-c0bc-4b93-843b-ed840637a2ee");

        // EXAMPLE USAGE

        [Fact]
        public void ExampleCommandHandler()
        {
            /* *************************************************************
             * This test demonstrates the action that might be taken in the
             * TransferMoneyCommand command handler
             * *************************************************************/

            // arrange
            var repository = GetFakeRepo();

            // simulating a command coming in to transfer from account A to B
            TransferMoneyCommand command = GetTransferRequest();

            // act
            var source = repository.Get(command.SourceAccountId);
            var destination = repository.Get(command.DestinationAccount);
            var amount = new Money(command.Amount);

            source.TransferOut(amount, destination, DateTimeOffset.UtcNow);
            repository.Save(source, destination);
        }


        // OTHER TESTS

        [Fact]
        public void Credit_PositiveAmount_GeneratesEvent()
        {
            var account = Account.Create();
            var amount = new Money(5.00M);
            account.Credit(amount, DateTimeOffset.UtcNow);

            AssertEventIsRaised<AccountCredited>(account);
        }

        [Fact]
        public void Credit_PositiveAmount_RaisesBalance()
        {
            var account = Account.Create();
            var amount = new Money(5.00M);
            account.Credit(amount, DateTimeOffset.UtcNow);

            Assert.Equal(amount.Amount, account.Balance.Amount);
        }

        [Theory]
        [InlineData(2.00, 3.00)]
        [InlineData(5.00, 1.00)]
        [InlineData(3.00, 6.00)]
        public void Credit_Addition_IsCorrect(decimal amount1, decimal amount2)
        {
            var a1 = new Money(amount1);
            var a2 = new Money(amount2);
            var expected = a1 + a2;
            var account = Account.Create();
            account.Credit(a1, DateTimeOffset.UtcNow);
            account.Credit(a2, DateTimeOffset.UtcNow);

            Assert.Equal(expected, account.Balance);
        }
        
        [Fact]
        public void Debit_SufficientFunds_GeneratesEvent()
        {
            var account = GetAccountA();
            var debitAmount = new Money(5.00M);
            account.Debit(debitAmount, DateTimeOffset.UtcNow);

            AssertEventIsRaised<AccountDebited>(account);
        }

        [Theory]
        [InlineData(2.00)]
        [InlineData(5.00)]
        [InlineData(3.00)]
        public void Debit_InsufficientFundsUnderLimit_AllowsNegative(decimal amountValue)
        {
            var account = Account.Create();
            var amount = new Money(amountValue);
            account.Debit(amount, DateTimeOffset.UtcNow);
            AssertEventIsRaised<AccountDebited>(account);
        }

        [Fact]
        public void Debit_InsufficientFundsOverLimit_SuspendsAccount()
        {
            var account = Account.Create();
            var amount = new Money(1000);
            account.Debit(amount, DateTimeOffset.UtcNow);

            AssertEventIsRaised<AccountSuspended>(account);
        }

        [Fact]
        public void Debit_SuspendedAccount_Throws()
        {
            var account = Account.Create();
            var amount = new Money(1000);

            // cause the account to become suspended
            account.Debit(amount, DateTimeOffset.UtcNow);

            // try to debit when suspended
            Assert.Throws<AccountSuspendedException>(() => account.Debit(amount, DateTimeOffset.UtcNow));
        }

        [Fact]
        public void Credit_AccountIsSuspended_NotYetZeroBalance_StillSuspended()
        {
            var account = Account.Create();
            var amount = new Money(1000);

            // cause the account to become suspended
            account.Debit(amount, DateTimeOffset.UtcNow);

            var creditAmount = new Money(5);
            account.Credit(creditAmount, DateTimeOffset.UtcNow);

            AssertEventIsNotRaised<AccountResumed>(account);
        }

        [Theory]
        [InlineData(1000, 1000)] // break even
        [InlineData(1000, 1001)] // more than break even
        [InlineData(500, 780)] // more than break even
        public void Credit_AccountIsSuspended_IsNowAboveZero_IsResumed(decimal debtAmount, decimal creditAmount)
        {
            var account = Account.Create();
            var debt = new Money(debtAmount);
            var repayment = new Money(creditAmount);

            // cause the account to become suspended
            account.Debit(debt, DateTimeOffset.UtcNow);

            // catch the account back up
            account.Credit(repayment, DateTimeOffset.UtcNow);

            AssertEventIsRaised<AccountResumed>(account);
        }

        [Fact]
        public void TransferOut_EachAccountHasCorrectEvents()
        {
            var accountA = GetAccountA();
            var accountB = GetAccountB();

            var transferAmount = new Money(10.00M);
            accountA.TransferOut(transferAmount, accountB, DateTimeOffset.UtcNow);

            AssertEventIsRaised<MoneyTransferredOut>(accountA);
            AssertEventIsRaised<MoneyTransferredIn>(accountB);
        }

        #region -- Helpers --
        private static void AssertEventIsRaised<TEvent>(AggregateRoot root)
        {
            var events = root.UncommittedEvents.ToList();
            var eventRaised = events.Any(e => e.GetType() == typeof(TEvent));
            Assert.True(eventRaised);
        }

        private static void AssertEventIsNotRaised<TEvent>(AggregateRoot root)
        {
            var events = root.UncommittedEvents.ToList();
            var eventRaised = events.Any(e => e.GetType() == typeof(TEvent));
            Assert.False(eventRaised);
        }

        private Account GetAccountA()
        {
            var accountA = Account.Create(_accountAid);
            accountA.Credit(new Money(3.00M), DateTimeOffset.UtcNow);
            accountA.Credit(new Money(5.00M), DateTimeOffset.UtcNow);
            accountA.Credit(new Money(8.00M), DateTimeOffset.UtcNow);
            accountA.Credit(new Money(13.00M), DateTimeOffset.UtcNow);
            return accountA;
        }

        private Account GetAccountB()
        {
            var accountB = Account.Create(_accountBid);
            accountB.Credit(new Money(21.00M), DateTimeOffset.UtcNow);
            accountB.Credit(new Money(34.00M), DateTimeOffset.UtcNow);
            accountB.Credit(new Money(55.00M), DateTimeOffset.UtcNow);
            accountB.Credit(new Money(89.00M), DateTimeOffset.UtcNow);
            return accountB;
        }
        
        private IRepository<Account> GetFakeRepo()
        {
            var accountA = GetAccountA();
            var accountB = GetAccountB();

            var repo = new AccountRepository();
            repo.Save(accountA);
            repo.Save(accountB);
            return repo;
        }

        private TransferMoneyCommand GetTransferRequest()
        {
            return new TransferMoneyCommand
            {
                SourceAccountId = _accountAid,
                DestinationAccount = _accountBid,
                Amount = 40
            };
        }

        public class TransferMoneyCommand
        {
            public Guid SourceAccountId { get; set; }

            public Guid DestinationAccount { get; set; }

            public decimal Amount { get; set; }
        }

        #endregion
    }
}
