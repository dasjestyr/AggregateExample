using System;

namespace AggregateExample3
{
    public interface IRepository<T>
        where T : AggregateRoot
    {
        T Get(Guid id);

        void Save(T entity);

        void Save(params T[] entities);
    }
}