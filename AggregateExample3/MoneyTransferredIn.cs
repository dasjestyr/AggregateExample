using System;

namespace AggregateExample3
{
    public class MoneyTransferredIn : Event
    {
        public decimal Amount { get; set; }

        public Guid SourceAccountId { get; set; }
        
        public DateTimeOffset TransferDate { get; set; }
    }
}