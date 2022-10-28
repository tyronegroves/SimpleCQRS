namespace EventSourcingCQRS.Eventing
{
    public interface IEventBus
    {
        void PublishEvent(DomainEvent domainEvent);
        void PublishEvents(IEnumerable<DomainEvent> domainEvents);
    }
}