namespace SimpleCqrs.Eventing
{
    public interface IHandleDomainEvents<in TDomainEvent> where TDomainEvent : DomainEvent
    {
        void Handle(TDomainEvent domainEvent);
    }
}