using EventSourcingCQRS.Eventing;
using Newtonsoft.Json.Linq;

namespace EventSourcingCQRS.EventStore.CosmosDb
{
    public interface IDomainEventSerializer
    {
        string Serialize(DomainEvent domainEvent);
        //DomainEvent Deserialize(Type targetType, string serializedDomainEvent);
        DomainEvent Deserialize(Type targetType, JObject serializedDomainEvent);
    }
}