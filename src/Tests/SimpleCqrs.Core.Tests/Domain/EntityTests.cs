using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleCqrs.Domain;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.Core.Tests.Domain
{
    [TestClass]
    public class EntityTests
    {
        [TestMethod]
        public void T()
        {
            //var domainRepository = new DomainRepository(new MemoryEventStore(), new NullSnapshotStore(), new LocalEventBus(new Type[] {}, new DomainEventHandlerFactory(null)));
            //var customerId = Guid.NewGuid();
            var orderId = Guid.NewGuid();

            //var customer = Customer.Create(customerId);

            //customer.AddOrder(orderId, "C00008334");
            //customer.CancelOrder(orderId);

            //domainRepository.Save(customer);

            var order = new Order(orderId, "D0000344");
            order.Cancel();
        }

        public class Order : Entity
        {
            private string orderNumber;
            private bool cancel;

            public Order(Guid orderId, string orderNumber)
            {
                Id = orderId;
                this.orderNumber = orderNumber;
            }

            public void Cancel()
            {
                Apply(new OrderCancelledEvent());
            }

            protected void OnOrderCancelled(OrderCancelledEvent domainEvent)
            {
                cancel = true;
            }
        }

        public class Customer : AggregateRoot
        {
            private Order order;

            public Customer()
            {
            }

            private Customer(Guid customerId)
            {
                Apply(new CustomerCreatedEvent {CustomerId = customerId});
            }

            public static Customer Create(Guid customerId)
            {
                return new Customer(customerId);
            }

            public void AddOrder(Guid orderId, string orderNumber)
            {
                Apply(new OrderCreatedEvent {OrderId = orderId, OrderNumber = orderNumber});
            }

            public void CancelOrder(Guid orderId)
            {
                order.Cancel();
            }

            protected void OnCustomerCreated(CustomerCreatedEvent domainEvent)
            {
                Id = domainEvent.CustomerId;
            }

            protected void OnOrderCreated(OrderCreatedEvent domainEvent)
            {
                order = new Order(domainEvent.OrderId, domainEvent.OrderNumber);
                RegisterEntity(order);
            }
        }
    }

    public class OrderCancelledEvent : EntityDomainEvent
    {
    }

    public class CustomerCreatedEvent : DomainEvent
    {
        public Guid CustomerId
        {
            get { return AggregateRootId; }
            set { AggregateRootId = value; }
        }
    }

    public class OrderCreatedEvent : EntityDomainEvent
    {
        public Guid OrderId
        {
            get { return EntityId; }
            set { EntityId = value; }
        }

        public string OrderNumber { get; set; }
    }
}