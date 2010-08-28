using System;

namespace SimpleCqrs.Domain
{
    public class Snapshot
    {
        public Guid AggregateRootId { get; set; }
        public int LastEventSequence { get; set; }
    }
}