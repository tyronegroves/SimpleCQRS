using System;
using Sample.JesseHouse.Events;
using SimpleCqrs.Eventing;

namespace Sample.JesseHouse.ViewModel
{
    public class CustomerDenormalizer :
        IHandleDomainEvents<CustomerHasBeenCreatedEvent>,
        IHandleDomainEvents<CustomerNameHasChangedEvent>
    {
        public void Handle(CustomerHasBeenCreatedEvent domainEvent)
        {
            Console.WriteLine("New customer created with ID: {0}", domainEvent.CustomerId);
        }

        public void Handle(CustomerNameHasChangedEvent domainEvent)
        {
            Console.WriteLine("Customer with ID: {0} name has changed.  New name is {1} {2}", domainEvent.CustomerId, domainEvent.FirstName, domainEvent.LastName);
        }
    }
}