using System;
using System.Collections.Generic;

namespace SimpleCqrs.Eventing
{
    public interface IEventStore
    {
        IEnumerable<DomainEvent> GetEvents(Guid aggregateRootId, int startSequence);
        void Insert(IEnumerable<DomainEvent> domainEvents);
        IEnumerable<DomainEvent> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes, DateTime startDate, DateTime endDate);
    }
}