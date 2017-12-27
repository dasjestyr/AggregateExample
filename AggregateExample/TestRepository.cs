using System;
using System.Collections.Generic;

namespace AggregateExample
{
    public class TestRepository<T> 
        where T : AggregateRoot, new()
    {
        private Dictionary<Guid, List<EventInfo>> _inMemoryStore = new Dictionary<Guid, List<EventInfo>>();

        public void Save(T aggregateRoot)
        {
            if(!_inMemoryStore.ContainsKey(aggregateRoot.Id))
                _inMemoryStore.Add(aggregateRoot.Id, new List<EventInfo>());

            _inMemoryStore[aggregateRoot.Id].AddRange(aggregateRoot.UncommittedEvents);
            aggregateRoot.ClearEvents();
        }

        public T GetById(Guid id)
        {
            if (!_inMemoryStore.ContainsKey(id))
                return null;

            var eventStream = _inMemoryStore[id];
            var entity = new T();
            foreach (var @event in eventStream)
                entity.InvokeHandler(@event);

            // THIS REPLAY METHOD WOULD HAVE TO SOMEHOW REPLAY ANY CHILD ENTITY STREAMS

            return entity;
        }

        public void Clear()
        {
            _inMemoryStore = new Dictionary<Guid, List<EventInfo>>();
        }
    }
}