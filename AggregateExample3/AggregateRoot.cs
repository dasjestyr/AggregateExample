using System.Collections.Generic;

namespace AggregateExample3
{
    public class AggregateRoot : Entity
    {
        private readonly List<Event> _uncommittedEvents = new List<Event>();

        public IReadOnlyCollection<Event> UncommittedEvents => _uncommittedEvents;

        protected void RaiseEvent(Event e)
        {
            _uncommittedEvents.Add(e);
        }

        public void ClearEvents()
        {
            _uncommittedEvents.Clear();
        }
    }
}