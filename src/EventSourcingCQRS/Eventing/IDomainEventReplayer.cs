namespace EventSourcingCQRS.Eventing
{
    public interface IDomainEventReplayer
    {
        Task ReplayEventsForHandlerType(Type handlerType, CancellationToken cancellationToken);
        Task ReplayEventsForHandlerType(Type handlerType, Guid aggregateRootId, CancellationToken cancellationToken);
        Task ReplayEventsForHandlerType(Type handlerType, Guid aggregateRootId, int lastSequenceNo, CancellationToken cancellationToken);
        Task ReplayEventsForHandlerType(Type handlerType, DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
    }
}