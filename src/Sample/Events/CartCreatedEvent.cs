using System;
using SimpleCqrs.Eventing;

namespace Events
{
    public class CartCreatedEvent : DomainEvent
    {
        public Guid Id { get; set; }
    }
}