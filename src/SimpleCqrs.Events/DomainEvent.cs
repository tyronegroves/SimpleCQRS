using System;

namespace SimpleCqrs.Events
{
    public class DomainEvent
    {
        public Guid AggregateRootId { get; set; }
        public int Sequence { get; set; }
    }
}