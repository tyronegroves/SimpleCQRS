using System;

namespace SimpleCqrs.Events
{
    public abstract class DomainEvent : IDomainEvent
    {
        public Guid AggregateRootId { get; set; }
        public int EventId { get; set; }
    }
}