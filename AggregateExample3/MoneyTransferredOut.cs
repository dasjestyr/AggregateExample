using System;

namespace AggregateExample3
{
    public class MoneyTransferredOut : Event
    {
        public decimal Amount { get; set; }

        public Guid DestinationAccountId { get; set; }

        public DateTimeOffset TransferDate { get; set; }
    }
}