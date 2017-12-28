using System;
using AggregateExample2.Infrastructure;

namespace AggregateExample2.Domain.Customer.Events
{
    public class SignedUp : EventInfo
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}