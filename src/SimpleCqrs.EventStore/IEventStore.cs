using System;
using System.Collections.Generic;
using SimpleCqrs.Events;

namespace SimpleCqrs.EventStore
{
    public interface IEventStore
    {
        IEnumerable<DomainEvent> GetAggregateRootEvents(Guid aggregateRootId);
        void Insert(IEnumerable<DomainEvent> domainEvents);
    }
}