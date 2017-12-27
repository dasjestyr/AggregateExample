using System;

namespace AggregateExample
{
    /// <summary>
    /// An entity that is a subordinate of the RootA-RootB-NonRootEntity aggregate, but is also the root of the RootB-NonRootEntity aggregate.
    /// </summary>
    public class RootB : AggregateRoot
    {
        private NonRootEntity _nonRootEntity;

        public RootB() : this(Guid.NewGuid())
        {
        }

        public RootB(Guid id)
        {
            Register<RootBCreatedEvent>(Apply);
            Register<RootBTestedEvent>(Apply);
            Register<NonRootEntityCreatedEvent>(Apply);
            Id = id;
        }
        
        public void Create()
        {
            var e = new RootBCreatedEvent { Message = "RootB Created from RootB"};
            AddEvent(e);
        }

        public void Test()
        {
            var e = new RootBTestedEvent {Message = "RootB tested from RootB"};
            AddEvent(e);
        }

        public void CreateTestEntity()
        {
            var e = new NonRootEntityCreatedEvent {Id = Guid.NewGuid(), Message = "NonRootEntity created from RootB"};
            AddEvent(e);
        }

        internal void Apply(RootBCreatedEvent obj)
        {
            Id = obj.Id;
            Console.WriteLine(obj.Message);
        }

        internal void Apply(RootBTestedEvent obj)
        {
            Console.WriteLine(obj.Message);
        }

        internal void Apply(NonRootEntityCreatedEvent obj)
        {
            _nonRootEntity = new NonRootEntity(obj.Id);
            Console.WriteLine(obj.Message);
        }
    }
}