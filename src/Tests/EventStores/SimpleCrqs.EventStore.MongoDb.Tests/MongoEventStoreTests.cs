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
            var eventStore = new MongoEventStore("Server=127.0.0.1");
            var repository = new DomainRepository(eventStore, new MyEventBus());

            var customer = new Customer();
            customer.Accept();
            customer.Deactivate();

            repository.Save(customer);

            var c = repository.GetById<Customer>(new Guid("ff4910ad-94bf-4862-b8d2-bb05e4fee46b"));
        }
    }

    public class Customer : AggregateRoot
    {
        private bool active;

        public void Accept()
        {
            PublishEvent(new CustomerAcceptedEvent { AggregateRootId = Guid.NewGuid() });
        }

        public void Deactivate()
        {
            PublishEvent(new CustomerDeactivitedEvent());
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