using System;

namespace AggregateExample
{
    public class RootBCreatedEvent : EventInfo
    {
        public Guid Id { get; set; }

        public string Message { get; set; }   
    }
}