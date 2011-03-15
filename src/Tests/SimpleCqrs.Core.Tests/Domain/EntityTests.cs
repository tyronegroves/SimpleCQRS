using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleCqrs.Domain;

namespace SimpleCqrs.Core.Tests.Domain
{
    [TestClass]
    public class EntityTests
    {
        [TestMethod]
        public void WhenAnEventIsAppliedToTheEntityItIsAssignedTheFirstEventSequence()
        {
            var aggregateRoot = new MyAggregateRoot();
            var entity = new MyEntity(Guid.NewGuid(), aggregateRoot);

            var domainEvent = (MyEntityCreatedEvent)aggregateRoot.UncommittedEvents.First();

        }

        [TestMethod]
        public void AggregateRootIsAssignedTheParentProperty()
        {
            var aggregateRoot = new MyAggregateRoot();
            var entity = new MyEntity(Guid.Empty, aggregateRoot);

            Assert.AreSame(entity.Parent, aggregateRoot);
        }

        public class MyEntity : Entity
        {
            public MyEntity(Guid aggregateRootId, MyAggregateRoot aggregateRoot) : base(aggregateRoot)
            {
                Apply(new MyEntityCreatedEvent());
            }
        }

        public class MyAggregateRoot : AggregateRoot
        {
        }
    }
}