namespace EventSourcingCQRS.Eventing
{
    public interface IHandleDomainEvents<in TDomainEvent> where TDomainEvent : DomainEvent
    {
        Task Handle(TDomainEvent domainEvent);
    }
}