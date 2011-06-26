using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SimpleCqrs.Domain;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.Core.Tests.Domain
{
    [TestClass]
    public class AggregateRootTests
    {
        private Mock<MyAggregateRoot> mockAggregateRoot;

        [TestInitialize]
        public void SetupMockForAllTests()
        {
            mockAggregateRoot = new Mock<MyAggregateRoot> {CallBase = true};
        }

        [TestMethod]
        public void WhenRegisterEntityIsCalledEntityAggegrateRootIsAssigned()
        {
            var aggregateRoot = new MyAggregateRoot();
            var entity = new MyEntity();

            aggregateRoot.RegisterEntity(entity);

            Assert.AreSame(aggregateRoot, entity.AggregateRoot);
        }

        [TestMethod]
        public void HandlerIsCalledWhenHandlerMeetsTheConventionAndEventIsApplied()
        {
            var domainEvent = new AggregateRootHandlerThatMeetsConventionEvent();
            var aggregateRoot = mockAggregateRoot.Object;

            aggregateRoot.Apply(domainEvent);

            mockAggregateRoot.Verify(ar => ar.OnAggregateRootHandlerThatMeetsConvention(domainEvent), Times.Once());
        }

        [TestMethod]
        public void HandlerIsNotCalledWhenHandlerDoesNotMeetTheConventionAndEventIsApplied()
        {
            var domainEvent = new AggregateRootHandlerThatMeetsConventionEvent();
            var aggregateRoot = mockAggregateRoot.Object;

            aggregateRoot.Apply(domainEvent);

            mockAggregateRoot.Verify(ar => ar.OnAggregateRootHandlerThatDoesNotMeetsConvention(domainEvent), Times.Never());
        }

        [TestMethod]
        public void HandlerIsNotCalledWhenHandlerHasTwoParametersAndEventIsApplied()
        {
            var domainEvent = new AggregateRootHandlerThatMeetsConventionEvent();
            var aggregateRoot = mockAggregateRoot.Object;

            aggregateRoot.Apply(domainEvent);

            mockAggregateRoot.Verify(ar => ar.OnAggregateRootHandlerThatMeetsConvention(domainEvent, "test"), Times.Never());
        }

        [TestMethod]
        public void HandlerIsCalledWhenHandlerIsPrivateAndEventIsApplied()
        {
            var domainEvent = new AggregateRootPrivateHandlerThatMeetsConventionEvent();
            var aggregateRoot = new MyAggregateRoot();

            aggregateRoot.Apply(domainEvent);

            Assert.IsTrue(aggregateRoot.OnAggregateRootPrivateHandlerThatMeetsConventionCalled);
        }

        [TestMethod]
        public void HandlerIsCalledWhenHandlerIsProtectedAndEventIsApplied()
        {
            var domainEvent = new AggregateRootProtectedHandlerThatMeetsConventionEvent();
            var aggregateRoot = new MyAggregateRoot();

            aggregateRoot.Apply(domainEvent);

            Assert.IsTrue(aggregateRoot.OnAggregateRootProtectedHandlerThatMeetsConventionCalled);
        }

        [TestMethod]
        public void UncommittedEventsAreClearedWhenCommitEventsIsCalled()
        {
            var aggregateRoot = mockAggregateRoot.Object;

            aggregateRoot.Apply(new AggregateRootHandlerThatMeetsConventionEvent());
            aggregateRoot.Apply(new AggregateRootHandlerThatMeetsConventionEvent());
            aggregateRoot.Apply(new AggregateRootHandlerThatMeetsConventionEvent());

            aggregateRoot.CommitEvents();

            Assert.AreEqual(0, aggregateRoot.UncommittedEvents.Count);
        }

        [TestMethod]
        public void DomainEventIsPlacedInTheUncommittedEventsPropertyIfPublished()
        {
            var domainEvent = new AggregateRootHandlerThatMeetsConventionEvent();
            var aggregateRoot = mockAggregateRoot.Object;

            aggregateRoot.Apply(domainEvent);

            Assert.AreSame(domainEvent, aggregateRoot.UncommittedEvents.First());
        }

        [TestMethod]
        public void MultipleDomainEventsArePlacedInTheUncommittedEventsPropertyInTheCorrectOrderIfPublished()
        {
            var domainEvent1 = new AggregateRootHandlerThatMeetsConventionEvent();
            var domainEvent2 = new AggregateRootHandlerThatMeetsConventionEvent();
            var domainEvent3 = new AggregateRootHandlerThatMeetsConventionEvent();
            var aggregateRoot = mockAggregateRoot.Object;

            aggregateRoot.Apply(domainEvent1);
            aggregateRoot.Apply(domainEvent2);
            aggregateRoot.Apply(domainEvent3);

            var uncommittedEvents = aggregateRoot.UncommittedEvents;
            Assert.AreEqual(3, uncommittedEvents.Count());
            Assert.AreSame(domainEvent1, uncommittedEvents[0]);
            Assert.AreSame(domainEvent2, uncommittedEvents[1]);
            Assert.AreSame(domainEvent3, uncommittedEvents[2]);
        }

        [TestMethod]
        public void EventThatIsPublishedIsAssignedTheNextEventId()
        {
            var domainEvent = new AggregateRootHandlerThatMeetsConventionEvent();
            var aggregateRoot = mockAggregateRoot.Object;

            aggregateRoot.LoadFromHistoricalEvents(new AggregateRootHandlerThatMeetsConventionEvent {Sequence = 203});
            aggregateRoot.Apply(domainEvent);

            Assert.AreEqual(204, domainEvent.Sequence);
        }

        [TestMethod]
        public void DomainEventsAreAssignedSequentialSequenceWhenPublished()
        {
            var domainEvent1 = new AggregateRootHandlerThatMeetsConventionEvent();
            var domainEvent2 = new AggregateRootHandlerThatMeetsConventionEvent();
            var domainEvent3 = new AggregateRootHandlerThatMeetsConventionEvent();
            var aggregateRoot = mockAggregateRoot.Object;

            aggregateRoot.Apply(domainEvent1);
            aggregateRoot.Apply(domainEvent2);
            aggregateRoot.Apply(domainEvent3);

            Assert.AreEqual(1, domainEvent1.Sequence);
            Assert.AreEqual(2, domainEvent2.Sequence);
            Assert.AreEqual(3, domainEvent3.Sequence);
        }

        [TestMethod]
        public void EventsAreSortedBySequenceBeforeBeingAppliedToTheAggregateRoot()
        {
            var aggregateRoot = new MyAggregateRoot();
            var domainEvents = new List<AggregateRootHandlerThatMeetsConventionEvent>
                                   {
                                       new AggregateRootHandlerThatMeetsConventionEvent {Sequence = 5},
                                       new AggregateRootHandlerThatMeetsConventionEvent {Sequence = 1},
                                       new AggregateRootHandlerThatMeetsConventionEvent {Sequence = 100},
                                       new AggregateRootHandlerThatMeetsConventionEvent {Sequence = 2}
                                   };

            aggregateRoot.LoadFromHistoricalEvents(domainEvents.ToArray());

            Assert.AreEqual(1, aggregateRoot.EventIds[0]);
            Assert.AreEqual(2, aggregateRoot.EventIds[1]);
            Assert.AreEqual(5, aggregateRoot.EventIds[2]);
            Assert.AreEqual(100, aggregateRoot.EventIds[3]);
        }

        [TestMethod]
        public void AggregateRootIdIsAssignedToTheEventWhenTheEventIsAppliedAndTheAggregateRootIdIsGuidEmpty()
        {
            var aggregateRoot = mockAggregateRoot.Object;
            var aggregateRootId = aggregateRoot.Id;

            aggregateRoot.Apply(new AggregateRootHandlerThatMeetsConventionEvent());
            aggregateRoot.Apply(new AggregateRootHandlerThatMeetsConventionEvent());
            aggregateRoot.Apply(new AggregateRootHandlerThatMeetsConventionEvent());

            var allAggregateRootIdsMatch = aggregateRoot.UncommittedEvents
                                                .All(domainEvent => domainEvent.AggregateRootId == aggregateRootId);
            Assert.IsTrue(allAggregateRootIdsMatch);
        }
    }

    public class MyEntity : Entity
    {
    }

    public class MyAggregateRoot : AggregateRoot
    {
        private int count;

        public MyAggregateRoot()
        {
            EventIds = new List<int>();
            Id = Guid.NewGuid();
        }

        public List<int> EventIds { get; private set; }
        public bool OnAggregateRootPrivateHandlerThatMeetsConventionCalled { get; set; }
        public bool OnAggregateRootProtectedHandlerThatMeetsConventionCalled { get; set; }

        public virtual void OnAggregateRootHandlerThatMeetsConvention(AggregateRootHandlerThatMeetsConventionEvent domainEvent)
        {
            EventIds.Add(domainEvent.Sequence);
        }

        public virtual void OnAggregateRootHandlerThatDoesNotMeetsConvention(AggregateRootHandlerThatMeetsConventionEvent domainEvent)
        {
        }

        public virtual void OnAggregateRootHandlerThatMeetsConvention(AggregateRootHandlerThatMeetsConventionEvent domainEvent, string test)
        {
        }

        private void OnAggregateRootPrivateHandlerThatMeetsConvention(AggregateRootPrivateHandlerThatMeetsConventionEvent domainEvent)
        {
            OnAggregateRootPrivateHandlerThatMeetsConventionCalled = true;
        }

        protected void OnAggregateRootProtectedHandlerThatMeetsConvention(AggregateRootProtectedHandlerThatMeetsConventionEvent domainEvent)
        {
            OnAggregateRootProtectedHandlerThatMeetsConventionCalled = true;
        }
    }

    public class AggregateRootHandlerThatMeetsConventionEvent : DomainEvent
    {
    }

    public class AggregateRootPrivateHandlerThatMeetsConventionEvent : DomainEvent
    {
    }

    public class AggregateRootProtectedHandlerThatMeetsConventionEvent : DomainEvent
    {
    }
}