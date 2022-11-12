using System.Diagnostics.CodeAnalysis;

namespace EventSourcingCQRS.Eventing
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class DomainEvent
    {
        public Guid AggregateRootId { get; set; }
        public int Sequence { get; set; }
        public DateTime EventDate { get; set; }
    }
}