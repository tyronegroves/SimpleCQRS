using System;
using System.Collections.Generic;
using System.Linq;
using AutoMoq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SimpleCqrs.Domain;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.Core.Tests.Events
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
                .Setup(eventStore => eventStore.GetEvents(aggregateRootId, 0))
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

            aggregateRoot.Apply(domainEvents[0]);
            aggregateRoot.Apply(domainEvents[1]);
            aggregateRoot.Apply(domainEvents[2]);

            repository.Save(aggregateRoot);

            mocker.GetMock<IEventStore>()
                .Verify(eventStore => eventStore.Insert(It.Is<IEnumerable<DomainEvent>>(events => events.All(domainEvents.Contains))), Times.Once());
        }

        [TestMethod]
        public void UncommittedEventsArePublishedToTheEventBusWhenSaved()
        {
            var repository = CreateDomainRepository();
            var aggregateRoot = new MyTestAggregateRoot();
            var domainEvents = new List<DomainEvent> {new MyTestEvent(), new MyTestEvent(), new MyTestEvent()};

            aggregateRoot.Apply(domainEvents[0]);
            aggregateRoot.Apply(domainEvents[1]);
            aggregateRoot.Apply(domainEvents[2]);

            repository.Save(aggregateRoot);

            mocker.GetMock<IEventBus>()
                .Verify(eventBus => eventBus.PublishEvents(It.IsAny<IEnumerable<DomainEvent>>()), Times.Once());
        }

        [TestMethod]
        public void UncommittedEventsShouldBeCommited()
        {
            var repository = CreateDomainRepository();
            var aggregateRoot = new MyTestAggregateRoot();

            aggregateRoot.Apply(new MyTestEvent());
            aggregateRoot.Apply(new MyTestEvent());

            repository.Save(aggregateRoot);

            Assert.AreEqual(0, aggregateRoot.UncommittedEvents.Count);
        }

        private DomainRepository CreateDomainRepository()
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

            private void OnMyTest(MyTestEvent myTestEvent)
            {
                MyTestEventHandleCount++;
                EventIds.Add(myTestEvent.Sequence);
            }
        }

        public class MyTestEvent : DomainEvent
        {
        }
    }
}