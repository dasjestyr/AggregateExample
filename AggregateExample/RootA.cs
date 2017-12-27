using System;

namespace AggregateExample
{
    public class RootA : AggregateRoot
    {
        private RootB _rootb;
        
        public RootA()
            : this(Guid.NewGuid())
        {
        }

        public RootA(Guid id)
        {
            Id = id;
            Register<RootBCreatedEvent>(Apply);
            Register<RootBTestedEvent>(Apply);
        }

        public void CreateB(Guid id)
        {
            var e = new RootBCreatedEvent {Message = "RootB created from RootA", Id = id};
            AddEvent(e);
        }

        public void TestB()
        {
            var e = new RootBTestedEvent{Message = "RootB tested from RootA"};
            AddEvent(e);
        }

        internal void Apply(RootBCreatedEvent e)
        {
            _rootb = new RootB();
            _rootb.Apply(e);
        }

        internal void Apply(RootBTestedEvent e)
        {
            _rootb.Apply(e);
        }
    }
}