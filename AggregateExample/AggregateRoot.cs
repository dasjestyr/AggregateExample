using System;
using System.Collections.Generic;

namespace AggregateExample
{
    public abstract class AggregateRoot : Entity
    {
        private readonly List<EventInfo> _uncommittedEvents = new List<EventInfo>();
        private readonly Dictionary<Type, Action<object>> _handlers = new Dictionary<Type, Action<object>>();
        
        public IReadOnlyCollection<EventInfo> UncommittedEvents => _uncommittedEvents;
        
        protected void AddEvent(EventInfo e)
        {
            _uncommittedEvents.Add(e);
            InvokeHandler(e);
        }

        protected void Register<T>(Action<T> handler)
             where T : EventInfo
        {
            _handlers.Add(typeof(T), arg => handler((T) arg));
        }

        public void ClearEvents()
        {
            _uncommittedEvents.Clear();
        }

        internal void InvokeHandler(EventInfo e)
        {
            var t = e.GetType();
            if(!_handlers.ContainsKey(t))
                throw new ArgumentException($"No handler was registered for {t}");

            _handlers[t].DynamicInvoke(e);
        }
    }
}
