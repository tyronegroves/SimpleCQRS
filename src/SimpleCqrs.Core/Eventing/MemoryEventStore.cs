using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCqrs.Eventing
{ 
    public class MemoryEventStore : IEventStore
    {
        private readonly List<DomainEvent> storedDomainEvents = new List<DomainEvent>();

        public IEnumerable<DomainEvent> GetEvents(Guid aggregateRootId, int startSequence)
        {
            return (from domainEvent in storedDomainEvents
                    where domainEvent.AggregateRootId == aggregateRootId
                    where domainEvent.Sequence > startSequence
                    select domainEvent).ToList();
        }

        public void Insert(IEnumerable<DomainEvent> domainEvents)
        {
            storedDomainEvents.AddRange(domainEvents);
        }

        public IEnumerable<DomainEvent> GetEventsOfTheseTypes(IEnumerable<Type> domainEventTypes)
        {
            throw new NotImplementedException();
        }
    }
}
