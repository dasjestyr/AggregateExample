using System;
using System.Collections.Generic;

namespace AggregateExample2.Infrastructure
{
    public class TestRepository<T> 
        where T : AggregateRoot
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
            var entity = CreateInstance();
            foreach (var @event in eventStream)
                entity.InvokeHandler(@event);
            
            return entity;
        }

        private static T CreateInstance()
        {
            var instance = Activator.CreateInstance(typeof(T), true);
            return (T) instance;
        }

        public void Clear()
        {
            _inMemoryStore = new Dictionary<Guid, List<EventInfo>>();
        }
    }
}