using System;

namespace SimpleCqrs.Events
{
    public interface IDomainEvent
    {
        Guid AggregateRootId { get; set; }
        int Sequence { get; set; }
    }
}