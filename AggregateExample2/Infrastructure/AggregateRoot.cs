using System;
using System.Collections.Generic;

namespace AggregateExample2.Infrastructure
{
    public abstract class AggregateRoot : Entity
    {
        private readonly List<EventInfo> _uncommittedEvents = new List<EventInfo>();
        private readonly Dictionary<Type, Action<object>> _handlers = new Dictionary<Type, Action<object>>();
        
        public IReadOnlyCollection<EventInfo> UncommittedEvents => _uncommittedEvents;
        
        protected void RegisterEventRoute<T>(Action<T> handler)
             where T : EventInfo
        {
            _handlers.Add(typeof(T), arg => handler((T) arg));
        }

        protected void RaiseEvent(EventInfo e)
        {
            _uncommittedEvents.Add(e);
            InvokeHandler(e);
        }

        protected void RaiseEvent<T>(Action<T> setup) 
            where T : EventInfo, new()
        {
            var e = new T();
            setup(e);
            RaiseEvent(e);
        }

        public void ApplyEvent(EventInfo e)
        {
            var t = e.GetType();
            if(_handlers.ContainsKey(t))
                throw new InvalidOperationException($"No handler found for {t}");

            _handlers[t].Invoke(e);
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
