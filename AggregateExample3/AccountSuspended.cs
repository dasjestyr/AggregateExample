using System;

namespace AggregateExample3
{
    public class AccountSuspended : Event
    {
        public DateTimeOffset SuspendedDate { get; set; }
    }
}