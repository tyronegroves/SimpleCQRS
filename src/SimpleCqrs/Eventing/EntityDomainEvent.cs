using System;

namespace SimpleCqrs.Eventing
{
    public class EntityDomainEvent : DomainEvent
    {
        public Guid EntityId { get; set; }
    }
}