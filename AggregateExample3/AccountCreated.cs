using System;

namespace AggregateExample3
{
    public class AccountCreated : Event
    {
        public Guid Id { get; set; }
    }
}