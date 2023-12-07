namespace EventSourcingCQRS.Eventing
{
    public interface IEventBus
    {
        Task Publish(DomainEvent domainEvent, CancellationToken cancellationToken);

        Task PublishEvents(IEnumerable<DomainEvent> domainEvents, CancellationToken cancellationToken);
    }
}