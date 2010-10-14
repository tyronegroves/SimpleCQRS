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
        public void HandlerIsCalledWhenHandlerMeetsTheConventionAndEventIsApplied()
        {
            var domainEvent = new HandlerThatMeetsConventionEvent();
            var aggregateRoot = mockAggregateRoot.Object;

            aggregateRoot.Apply(domainEvent);

            mockAggregateRoot.Verify(ar => ar.OnHandlerThatMeetsConvention(domainEvent), Times.Once());
        }

        [TestMethod]
        public void HandlerIsNotCalledWhenHandlerDoesNotMeetTheConventionAndEventIsApplied()
        {
            var domainEvent = new HandlerThatMeetsConventionEvent();
            var aggregateRoot = mockAggregateRoot.Object;

            aggregateRoot.Apply(domainEvent);

            mockAggregateRoot.Verify(ar => ar.OnHandlerThatDoesNotMeetsConvention(domainEvent), Times.Never());
        }

        [TestMethod]
        public void HandlerIsNotCalledWhenHandlerHasTwoParametersAndEventIsApplied()
        {
            var domainEvent = new HandlerThatMeetsConventionEvent();
            var aggregateRoot = mockAggregateRoot.Object;

            aggregateRoot.Apply(domainEvent);

            mockAggregateRoot.Verify(ar => ar.OnHandlerThatMeetsConvention(domainEvent, "test"), Times.Never());
        }

        [TestMethod]
        public void HandlerIsCalledWhenHandlerIsPrivateAndEventIsApplied()
        {
            var domainEvent = new PrivateHandlerThatMeetsConventionEvent();
            var aggregateRoot = new MyAggregateRoot();

            aggregateRoot.Apply(domainEvent);

            Assert.IsTrue(aggregateRoot.OnPrivateHandlerThatMeetsConventionCalled);
        }

        [TestMethod]
        public void HandlerIsCalledWhenHandlerIsProtectedAndEventIsApplied()
        {
            var domainEvent = new ProtectedHandlerThatMeetsConventionEvent();
            var aggregateRoot = new MyAggregateRoot();

            aggregateRoot.Apply(domainEvent);

            Assert.IsTrue(aggregateRoot.OnProtectedHandlerThatMeetsConventionCalled);
        }

        [TestMethod]
        public void UncommittedEventsAreClearedWhenCommitEventsIsCalled()
        {
            var aggregateRoot = mockAggregateRoot.Object;

            aggregateRoot.Apply(new HandlerThatMeetsConventionEvent());
            aggregateRoot.Apply(new HandlerThatMeetsConventionEvent());
            aggregateRoot.Apply(new HandlerThatMeetsConventionEvent());

            aggregateRoot.CommitEvents();

            Assert.AreEqual(0, aggregateRoot.UncommittedEvents.Count);
        }

        [TestMethod]
        public void DomainEventIsPlacedInTheUncommittedEventsPropertyIfPublished()
        {
            var domainEvent = new HandlerThatMeetsConventionEvent();
            var aggregateRoot = mockAggregateRoot.Object;

            aggregateRoot.Apply(domainEvent);

            Assert.AreSame(domainEvent, aggregateRoot.UncommittedEvents.First());
        }

        [TestMethod]
        public void MultipleDomainEventsArePlacedInTheUncommittedEventsPropertyInTheCorrectOrderIfPublished()
        {
            var domainEvent1 = new HandlerThatMeetsConventionEvent();
            var domainEvent2 = new HandlerThatMeetsConventionEvent();
            var domainEvent3 = new HandlerThatMeetsConventionEvent();
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
            var domainEvent = new HandlerThatMeetsConventionEvent();
            var aggregateRoot = mockAggregateRoot.Object;

            aggregateRoot.LoadFromHistoricalEvents(new HandlerThatMeetsConventionEvent {Sequence = 203});
            aggregateRoot.Apply(domainEvent);

            Assert.AreEqual(204, domainEvent.Sequence);
        }

        [TestMethod]
        public void DomainEventsAreAssignedSequentialSequenceWhenPublished()
        {
            var domainEvent1 = new HandlerThatMeetsConventionEvent();
            var domainEvent2 = new HandlerThatMeetsConventionEvent();
            var domainEvent3 = new HandlerThatMeetsConventionEvent();
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
            var domainEvents = new List<HandlerThatMeetsConventionEvent>
                                   {
                                       new HandlerThatMeetsConventionEvent {Sequence = 5},
                                       new HandlerThatMeetsConventionEvent {Sequence = 1},
                                       new HandlerThatMeetsConventionEvent {Sequence = 100},
                                       new HandlerThatMeetsConventionEvent {Sequence = 2}
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

            aggregateRoot.Apply(new HandlerThatMeetsConventionEvent());
            aggregateRoot.Apply(new HandlerThatMeetsConventionEvent());
            aggregateRoot.Apply(new HandlerThatMeetsConventionEvent());

            var allAggregateRootIdsMatch = aggregateRoot.UncommittedEvents
                                                .All(domainEvent => domainEvent.AggregateRootId == aggregateRootId);
            Assert.IsTrue(allAggregateRootIdsMatch);
        }
    }

    public class MyAggregateRoot : AggregateRoot
    {
        public MyAggregateRoot()
        {
            EventIds = new List<int>();
            Id = Guid.NewGuid();
        }

        public List<int> EventIds { get; private set; }
        public bool OnPrivateHandlerThatMeetsConventionCalled { get; set; }
        public bool OnProtectedHandlerThatMeetsConventionCalled { get; set; }

        public virtual void OnHandlerThatMeetsConvention(HandlerThatMeetsConventionEvent domainEvent)
        {
            EventIds.Add(domainEvent.Sequence);
        }

        public virtual void OnHandlerThatDoesNotMeetsConvention(HandlerThatMeetsConventionEvent domainEvent)
        {
        }

        public virtual void OnHandlerThatMeetsConvention(HandlerThatMeetsConventionEvent domainEvent, string test)
        {
        }

        private void OnPrivateHandlerThatMeetsConvention(PrivateHandlerThatMeetsConventionEvent domainEvent)
        {
            OnPrivateHandlerThatMeetsConventionCalled = true;
        }

        protected void OnProtectedHandlerThatMeetsConvention(ProtectedHandlerThatMeetsConventionEvent domainEvent)
        {
            OnProtectedHandlerThatMeetsConventionCalled = true;
        }
    }

    public class HandlerThatMeetsConventionEvent : DomainEvent
    {
    }

    public class PrivateHandlerThatMeetsConventionEvent : DomainEvent
    {
    }

    public class ProtectedHandlerThatMeetsConventionEvent : DomainEvent
    {
    }
}