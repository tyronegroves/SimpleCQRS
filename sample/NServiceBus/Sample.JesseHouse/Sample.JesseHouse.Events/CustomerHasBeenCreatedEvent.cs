using System;
using SimpleCqrs.Eventing;

namespace Sample.JesseHouse.Events
{
    [Serializable]
    public class CustomerHasBeenCreatedEvent : DomainEvent
    {
        public Guid CustomerId
        {
            get { return AggregateRootId; }
            set { AggregateRootId = value; }
        }
    }
}