using System;

namespace AggregateExample3
{
    public class Entity
    {
        public Guid Id { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Entity);
        }

        protected bool Equals(Entity other)
        {
            return other != null && Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}