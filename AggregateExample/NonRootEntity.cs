using System;

namespace AggregateExample
{
    /// <summary>
    /// An entity that is never used as an aggregate root.
    /// </summary>
    public class NonRootEntity : Entity
    {
        public NonRootEntity() : this(Guid.NewGuid())
        {
        }

        public NonRootEntity(Guid id)
        {
            Id = id;
        }
    }
}