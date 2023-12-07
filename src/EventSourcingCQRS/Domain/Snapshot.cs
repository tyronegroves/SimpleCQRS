using System.Diagnostics.CodeAnalysis;

namespace EventSourcingCQRS.Domain
{
    [ExcludeFromCodeCoverage]
    public class Snapshot
    {
        public Guid AggregateRootId { get; set; }
        public int LastEventSequence { get; set; }
    }
}