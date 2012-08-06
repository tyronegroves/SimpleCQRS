using System;

namespace SimpleCqrs.Eventing
{
    [Serializable]
    public class EntityDomainEvent : DomainEvent
    {
        public Guid EntityId { get; set; }
    }
}