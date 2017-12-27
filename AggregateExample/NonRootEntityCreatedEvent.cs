using System;

namespace AggregateExample
{
    public class NonRootEntityCreatedEvent : EventInfo
    {
        public Guid Id { get; set; }

        public string Message { get; set; }
    }
}