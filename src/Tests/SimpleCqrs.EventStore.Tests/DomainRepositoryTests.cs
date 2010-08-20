using System;
using System.Collections.Generic;
using System.Linq;
using AutoMoq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SimpleCqrs.Domain;
using SimpleCqrs.Events;

namespace SimpleCqrs.EventStore.Tests
{
    [TestClass]
    public class DomainRepositoryTests
    {
        private AutoMoqer mocker;

        [TestInitialize]
        public void SetupMocksForAllTests()
        {
            mocker = new AutoMoqer();
        }

        [TestMethod]
        public void EventsAreRetrievedFromTheEventStoreAndAppliedToTheAggregateRootWhenGetByIdIsCalled()
        {
            var repository = CreateDomainRepository();
            var aggregateRootId = new Guid();
            var domainEvents = new List<MyTestEvent> {new MyTestEvent(), new MyTestEvent(), new MyTestEvent()};

            mocker.GetMock<IEventStore>()
                .Setup(eventStore => eventStore.GetAggregateEvents(aggregateRootId))
                .Returns(domainEvents);

            var aggregateRoot = repository.GetById<MyTestAggregateRoot>(aggregateRootId);

            Assert.AreEqual(3, aggregateRoot.MyTestEventHandleCount);
        }

        [TestMethod]
        public void UncommittedEventsAreInsertedIntoTheEventStoreWhenSaved()
        {
            var repository = CreateDomainRepository();
            var aggregateRoot = new MyTestAggregateRoot();
            var domainEvents = new List<DomainEvent> {new MyTestEvent(), new MyTestEvent(), new MyTestEvent()};
            
            aggregateRoot.PublishDomainEvent(domainEvents[0]);
            aggregateRoot.PublishDomainEvent(domainEvents[1]);
            aggregateRoot.PublishDomainEvent(domainEvents[2]);

            repository.Save(aggregateRoot);

            mocker.GetMock<IEventStore>()
                .Verify(eventStore => eventStore.Insert(It.Is<IEnumerable<IDomainEvent>>(events => events.All(domainEvents.Contains))), Times.Once());
        }

        [TestMethod]
        public void UncommittedEventsArePublishedToTheEventBusWhenSaved()
        {
            var repository = CreateDomainRepository();
            var aggregateRoot = new MyTestAggregateRoot();
            var domainEvents = new List<DomainEvent> { new MyTestEvent(), new MyTestEvent(), new MyTestEvent() };

            aggregateRoot.PublishDomainEvent(domainEvents[0]);
            aggregateRoot.PublishDomainEvent(domainEvents[1]);
            aggregateRoot.PublishDomainEvent(domainEvents[2]);

            repository.Save(aggregateRoot);

            mocker.GetMock<IEventBus>()
                .Verify(eventBus => eventBus.PublishEvents(It.IsAny<IEnumerable<IDomainEvent>>()), Times.Once());
        }

        [TestMethod]
        public void UncommittedEventsShouldBeCommited()
        {
            var repository = CreateDomainRepository();
            var aggregateRoot = new MyTestAggregateRoot();

            aggregateRoot.PublishDomainEvent(new MyTestEvent());
            aggregateRoot.PublishDomainEvent(new MyTestEvent());

            repository.Save(aggregateRoot);

            Assert.AreEqual(0, aggregateRoot.UncommittedEvents.Count);
        }

        public DomainRepository CreateDomainRepository()
        {
            return mocker.Resolve<DomainRepository>();
        }

        public class MyTestAggregateRoot : AggregateRoot
        {
            public int MyTestEventHandleCount { get; set; }
            public List<int> EventIds { get; set; }

            public MyTestAggregateRoot()
            {
                EventIds = new List<int>();
            }

            private void OnMyTestEvent(MyTestEvent myTestEvent)
            {
                MyTestEventHandleCount++;
                EventIds.Add(myTestEvent.Sequence);
            }

            public void PublishDomainEvent(DomainEvent domainEvent)
            {
                PublishEvent(domainEvent);
            }
        }

        public class MyTestEvent : DomainEvent
        {
        }
    }
}