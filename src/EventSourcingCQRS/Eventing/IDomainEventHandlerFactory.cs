namespace EventSourcingCQRS.Eventing
{
    public interface IDomainEventHandlerFactory
    {
        object Create(Type domainEventHandlerType);
    }
}