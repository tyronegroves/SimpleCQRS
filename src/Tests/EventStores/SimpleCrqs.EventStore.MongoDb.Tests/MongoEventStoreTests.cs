using System;
using System.Collections.Generic;
using AutoMoq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleCqrs.Core;
using SimpleCqrs.Domain;
using SimpleCqrs.Events;
using SimpleCqrs.EventStore;
using SimpleCqrs.EventStore.MongoDb;

namespace SimpleCrqs.EventStore.MongoDb.Tests
{
    [TestClass]
    public class MongoEventStoreTests
    {
        [TestMethod]
        public void Test()
        {
            var eventStore = new MongoEventStore("Server=127.0.0.1", new MyDomainEventTypeCatalog());
            var repository = new DomainRepository(eventStore, new MyEventBus());

            var customer = new Customer();
            customer.Accept();
            customer.Deactivate();

            repository.Save(customer);
        }
    }

    public class Customer : AggregateRoot
    {
        private bool active;

        public void Accept()
        {
            Apply(new CustomerAcceptedEvent { AggregateRootId = Guid.NewGuid() });
        }

        public void Deactivate()
        {
            Apply(new CustomerDeactivitedEvent());
        }

        public void OnCustomerAcceptedEvent(CustomerAcceptedEvent customerAcceptedEvent)
        {
            Id = customerAcceptedEvent.AggregateRootId;
        }

        public void OnCustomerDeactivitedEvent(CustomerDeactivitedEvent customerDeactivitedEvent)
        {
            active = false;
        }
    }

    public class MyDomainEventTypeCatalog : IDomainEventTypeCatalog
    {
        public IEnumerable<Type> GetAllEventTypes()
        {
            return new[] {typeof(CustomerAcceptedEvent), typeof(CustomerDeactivitedEvent)};
        }
    }

    public class MyEventBus : IEventBus
    {
        public void PublishEvent(DomainEvent domainEvent)
        {
        }

        public void PublishEvents(IEnumerable<DomainEvent> domainEvents)
        {
        }
    }

    public class CustomerAcceptedEvent : DomainEvent
    {
    }

    public class CustomerDeactivitedEvent : DomainEvent
    {
    }
}