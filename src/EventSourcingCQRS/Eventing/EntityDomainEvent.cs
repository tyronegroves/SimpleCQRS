using System.Diagnostics.CodeAnalysis;

namespace EventSourcingCQRS.Eventing
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class EntityDomainEvent : DomainEvent
    {
        public Guid EntityId { get; set; }
    }
}