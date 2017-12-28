using AggregateExample2.Infrastructure;

namespace AggregateExample2.Domain.Customer.Events
{
    public class DepositedMoney : EventInfo
    {
        public decimal Amount { get; set; }
    }
}