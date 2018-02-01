using System;

namespace AggregateExample3
{
    public class AccountCredited : Event
    {
        public DateTimeOffset TransactionDate { get; set; }

        public decimal Amount { get; set; }
    }
}