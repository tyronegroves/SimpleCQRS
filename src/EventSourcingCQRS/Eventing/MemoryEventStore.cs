namespace EventSourcingCQRS.Eventing
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

        public IEnumerable<DomainEvent> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes)
        {
            return (from domainEvent in storedDomainEvents
                    where domainEventTypes.Contains(domainEvent.GetType())
                    select domainEvent);
        }

        public IEnumerable<DomainEvent> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes, Guid aggregateRootId)
        {
            return (from domainEvent in storedDomainEvents
                    where domainEvent.AggregateRootId == aggregateRootId
                    where domainEventTypes.Contains(domainEvent.GetType())
                    select domainEvent);
        }

        public IEnumerable<DomainEvent> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes, DateTime startDate, DateTime endDate)
        {
            return (from domainEvent in storedDomainEvents
                    where domainEvent.EventDate >= startDate
                    where domainEvent.EventDate <= endDate
                    where domainEventTypes.Contains(domainEvent.GetType())
                    select domainEvent);
        }
    }
}