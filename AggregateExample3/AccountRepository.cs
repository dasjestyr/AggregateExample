using System;
using System.Collections.Generic;

namespace AggregateExample3
{
    public class AccountRepository : IRepository<Account>
    {
        private static readonly Dictionary<Guid, Account> InMemoryStore = new Dictionary<Guid, Account>();

        public Account Get(Guid id)
        {
            // simplified for brevity. In an ES solution, this would retrieve
            // the stream and apply each event to the aggregate root. The
            // aggregate root would proxy any events to subordinate entities
            // who would then apply the events to themselves in order to 
            // update state
            return InMemoryStore.ContainsKey(id) ? InMemoryStore[id] : null;
        }

        public void Save(Account entity)
        {
            // this is simplified for brevity. In an ES solution, this
            // would pull out uncommitted events and persist them to the stream

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