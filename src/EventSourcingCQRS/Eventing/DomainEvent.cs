using System.Diagnostics.CodeAnalysis;

namespace EventSourcingCQRS.Eventing
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    public partial class DomainEvent
    {
        public Guid AggregateRootId { get; set; }
        public int Sequence { get; set; }
        public DateTimeOffset EventDate { get; set; }
    }
}