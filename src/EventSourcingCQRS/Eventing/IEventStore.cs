namespace EventSourcingCQRS.Eventing
{
    public interface IEventStore
    {
        Task<IEnumerable<Guid>> GetDistinctAggregateRootsId();
        Task<IEnumerable<DomainEvent>> GetEvents(Guid aggregateRootId, int startSequence);

        Task<IEnumerable<DomainEvent>> GetEventsUpToSequence(Guid aggregateRootId, int sequence);

        Task Insert(IEnumerable<DomainEvent> domainEvents);

        Task<IEnumerable<DomainEvent>> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes);

        Task<IEnumerable<DomainEvent>> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes, Guid aggregateRootId);
        Task<IEnumerable<DomainEvent>> GetEventsByEventTypesUpToSequence(IEnumerable<Type> domainEventTypes, Guid aggregateRootId, int sequence);

        Task<IEnumerable<DomainEvent>> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes, DateTime startDate,
            DateTime endDate);

        Task<IEnumerable<DomainEvent>> GetEventsByCriteria(Dictionary<string, string> criteria);
        Task<IEnumerable<DomainEvent>> GetEventsByCriteria(List<EventQueryCriteria> criteria);
    }
}