using EventSourcingCQRS.Eventing;
//using SimpleCqrs.Eventing;

namespace EventSourcingCQRS.EventStore.SqlServer
{
    public interface IDomainEventSerializer
    {
        string Serialize(DomainEvent domainEvent);
        DomainEvent Deserialize(Type targetType, string serializedDomainEvent);
    }
}
