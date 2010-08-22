using System;
using System.Collections.Generic;
using SimpleCqrs.Events;

namespace SimpleCqrs.EventStore
{
    public interface IEventStore
    {
        IEnumerable<DomainEvent> GetEvents(Guid aggregateRootId, int startSequence);
        void Insert(IEnumerable<DomainEvent> domainEvents);
    }
}