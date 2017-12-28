using AggregateExample2.Infrastructure;

namespace AggregateExample2.Domain.Customer.Events
{
    public class WalletCreated : EventInfo
    {
        public decimal InitialBalance { get; set; }
    }
}