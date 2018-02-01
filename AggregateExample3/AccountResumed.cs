using System;

namespace AggregateExample3
{
    public class AccountResumed : Event
    {
        public DateTimeOffset ResumedDate { get; set; }
    }
}