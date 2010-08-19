using System;
using System.Collections.Generic;
using SimpleCqrs.Events;

namespace SimpleCqrs.EventStore
{
    public interface IEventStore
    {
        IEnumerable<IDomainEvent> GetAggregateEvents(Guid aggregateRootId);
        void Insert(IEnumerable<IDomainEvent> domainEvents);
    }
}