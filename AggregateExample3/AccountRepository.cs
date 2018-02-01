using System;
using System.Collections.Generic;

namespace AggregateExample3
{
    public class AccountRepository : IRepository<Account>
    {
        private static readonly Dictionary<Guid, Account> InMemoryStore = new Dictionary<Guid, Account>();

        public Account Get(Guid id)
        {
            return InMemoryStore.ContainsKey(id) ? InMemoryStore[id] : null;
        }

        public void Save(Account entity)
        {
            if (InMemoryStore.ContainsKey(entity.Id))
            {
                InMemoryStore[entity.Id] = entity;
            }
            else
            {
                InMemoryStore.Add(entity.Id, entity);
            }
        }

        public void Save(params Account[] entities)
        {
            foreach(var entity in entities)
                Save(entity);
        }
    }
}