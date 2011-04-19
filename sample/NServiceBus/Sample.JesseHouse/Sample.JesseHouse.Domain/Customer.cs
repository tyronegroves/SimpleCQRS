using System;
using Sample.JesseHouse.Events;
using SimpleCqrs.Domain;

namespace Sample.JesseHouse.Domain
{
    public class Customer : AggregateRoot
    {
        public Customer(Guid customerId)
        {
            Apply(new CustomerHasBeenCreatedEvent{CustomerId = customerId});
        }

        public void SetName(string firstName, string lastName)
        {
            Apply(new CustomerNameHasChangedEvent{FirstName = firstName, LastName = lastName});
        }

        protected void OnCustomerHasBeenCreated(CustomerHasBeenCreatedEvent domainEvent)
        {
            Id = domainEvent.CustomerId;
        }
    }
}