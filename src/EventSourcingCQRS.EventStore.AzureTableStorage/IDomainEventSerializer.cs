using EventSourcingCQRS.Eventing;

namespace EventSourcingCQRS.EventStore.AzureTableStorage
{
    public interface IDomainEventSerializer
    {
        string Serialize(DomainEvent domainEvent);
        DomainEvent Deserialize(Type targetType, string serializedDomainEvent);
    }
}