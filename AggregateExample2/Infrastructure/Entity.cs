using System;

namespace AggregateExample2.Infrastructure
{
    public interface IEntity
    {
        Guid Id { get; }
    }

    public abstract class Entity : IEntity
    {
        public Guid Id { get; protected set; }
    }
}