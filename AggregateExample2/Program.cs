using System;
using AggregateExample2.Domain;
using AggregateExample2.Domain.Customer;
using AggregateExample2.Infrastructure;

namespace AggregateExample2
{
    public class Program
    {
        /// <summary>
        /// This is just to facilitate the demonstration. Normally, the wallet ID would be retrieved through the Query model 
        /// by the client and passed in as a property of the command.
        /// </summary>
        public static class TemporaryWalletValue
        {
            public static WalletId LastWalletId { get; set; }
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("-- INITIAL RUN --");
            var customer = Customer.Create("Jeremy");
            var repo = new TestRepository<Customer>();

            // Wallet A
            customer.CreateWallet(new Money(20));
            var walletAId = TemporaryWalletValue.LastWalletId;

            // Deposit to Wallet A
            var amountA = new Money(5.00M);
            customer.DespositMoney(walletAId, amountA);

            // Withdraw from Wallet A
            customer.WithdrawMoney(walletAId, amountA);

            // Withdraw too much from Wallet A
            try
            {
                customer.WithdrawMoney(walletAId, new Money(21.00M));
            }
            catch (InsufficientFundsException ex)
            {
                Console.WriteLine($"The AR refused the command. {ex.Message}");
            }

            // Create Wallet B
            customer.CreateWallet(new Money(5));
            var walletBId = TemporaryWalletValue.LastWalletId;

            // Withdraw from Wallet B
            customer.WithdrawMoney(walletBId, new Money(2.50M));

            // transfer all to wallet A
            customer.TransferToWallet(walletBId, walletAId, new Money(2.50M));

            repo.Save(customer);
            Console.WriteLine(Environment.NewLine);

            Console.WriteLine("-- REPLAY. OUTPUT SHOULD BE THE SAME BUT WITH NO ERROR --");
            var replayedCustomer = repo.GetById(customer.Id);

            Console.ReadKey();
        }
    }
}
