using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SimpleCqrs.Domain;
using SimpleCqrs.Eventing;
using SimpleCqrs.Testing;

namespace SimpleCqrs.Core.Tests.Domain
{
    [TestClass]
    public class EntityTests
    {
        private Mock<MyEntity> mockEntity;

        [TestInitialize]
        public void SetupMockForAllTests()
        {
            mockEntity = new Mock<MyEntity>(new Guid()) { CallBase = true };
        }

        [TestMethod]
        public void HandlerIsCalledWhenHandlerMeetsTheConventionAndEventIsApplied()
        {
            var domainEvent = new EntityHandlerThatMeetsConventionEvent();
            var entity = mockEntity.Object;
            entity.TurnTestModeOn();

            entity.Apply(domainEvent);

            mockEntity.Verify(ar => ar.OnEntityHandlerThatMeetsConvention(domainEvent), Times.Once());
        }

        [TestMethod]
        public void HandlerIsNotCalledWhenHandlerDoesNotMeetTheConventionAndEventIsApplied()
        {
            var domainEvent = new EntityHandlerThatMeetsConventionEvent();
            var entity = mockEntity.Object;
            entity.TurnTestModeOn();

            entity.Apply(domainEvent);

            mockEntity.Verify(ar => ar.OnEntityHandlerThatDoesNotMeetsConvention(domainEvent), Times.Never());
        }

        [TestMethod]
        public void HandlerIsNotCalledWhenHandlerHasTwoParametersAndEventIsApplied()
        {
            var domainEvent = new EntityHandlerThatMeetsConventionEvent();
            var entity = mockEntity.Object;
            entity.TurnTestModeOn();

            entity.Apply(domainEvent);

            mockEntity.Verify(ar => ar.OnEntityHandlerThatMeetsConvention(domainEvent, "test"), Times.Never());
        }

        [TestMethod]
        public void HandlerIsCalledWhenHandlerIsPrivateAndEventIsApplied()
        {
            var domainEvent = new EntityPrivateHandlerThatMeetsConventionEvent();
            var entity = new MyEntity(new Guid());
            entity.TurnTestModeOn();

            entity.Apply(domainEvent);

            Assert.IsTrue(entity.OnEntityPrivateHandlerThatMeetsConventionCalled);
        }

        [TestMethod]
        public void HandlerIsCalledWhenHandlerIsProtectedAndEventIsApplied()
        {
            var domainEvent = new EntityProtectedHandlerThatMeetsConventionEvent();
            var entity = new MyEntity(new Guid());
            entity.TurnTestModeOn();

            entity.Apply(domainEvent);

            Assert.IsTrue(entity.OnEntityProtectedHandlerThatMeetsConventionCalled);
        }

        [TestMethod]
        public void When_an_event_is_applied_to_the_entity_the_event_is_added_to_the_uncommittedevents()
        {
            var entityId = Guid.NewGuid();
            var entity = new MyEntity(entityId);
            entity.AggregateRoot = new MyAggregateRoot();
            entity.TurnTestModeOn();

            var entityCreatedEvent = new MyEntityCreatedEvent();

            entity.Apply(entityCreatedEvent);

            Assert.AreSame(entity.UncommittedEvents[0], entityCreatedEvent);
        }

        [TestMethod]
        public void When_an_event_is_applied_to_the_entity_the_event_is_applied_to_the_aggregate_root()
        {
            var entityId = Guid.NewGuid();
            var entity = new MyEntity(entityId);
            entity.AggregateRoot = new MyAggregateRoot();
            entity.TurnTestModeOn();

            entity.Apply(new MyEntityCreatedEvent());

            Assert.AreEqual(entity.UncommittedEvents[0].EntityId, entityId);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void If_test_mode_is_off_an_null_reference_exception_is_thrown_when_an_event_is_applied()
        {
            var entityId = Guid.NewGuid();
            var entity = new MyEntity(entityId);
            entity.TurnTestModeOff();

            entity.Apply(new MyEntityCreatedEvent());
        }

        [TestMethod]
        public void EntityId_is_set_on_applied_events()
        {
            var entityId = Guid.NewGuid();
            var entity = new MyEntity(entityId);
            entity.TurnTestModeOn();
            
            entity.Apply(new MyEntityCreatedEvent());

            Assert.AreEqual(entity.UncommittedEvents[0].EntityId, entityId);
        }

        public class MyEntity : Entity
        {
            public List<int> EventIds { get; private set; }
            public bool OnEntityPrivateHandlerThatMeetsConventionCalled { get; set; }
            public bool OnEntityProtectedHandlerThatMeetsConventionCalled { get; set; }
            
            public MyEntity(Guid entityId)
            {
                EventIds = new List<int>();
                Id = entityId;
            }

            public virtual void OnEntityHandlerThatMeetsConvention(EntityHandlerThatMeetsConventionEvent domainEvent)
            {
                EventIds.Add(domainEvent.Sequence);
            }

            public virtual void OnEntityHandlerThatDoesNotMeetsConvention(EntityHandlerThatMeetsConventionEvent domainEvent)
            {
            }

            public virtual void OnEntityHandlerThatMeetsConvention(EntityHandlerThatMeetsConventionEvent domainEvent, string test)
            {
            }

            private void OnEntityPrivateHandlerThatMeetsConvention(EntityPrivateHandlerThatMeetsConventionEvent domainEvent)
            {
                OnEntityPrivateHandlerThatMeetsConventionCalled = true;
            }

            protected void OnEntityProtectedHandlerThatMeetsConvention(EntityProtectedHandlerThatMeetsConventionEvent domainEvent)
            {
                OnEntityProtectedHandlerThatMeetsConventionCalled = true;
            }
        }

        public class MyEntityCreatedEvent : EntityDomainEvent
        {
        }

        public class EntityHandlerThatMeetsConventionEvent : EntityDomainEvent
        {
        }

        public class EntityPrivateHandlerThatMeetsConventionEvent : EntityDomainEvent
        {
        }

        public class EntityProtectedHandlerThatMeetsConventionEvent : EntityDomainEvent
        {
        }
    }
}