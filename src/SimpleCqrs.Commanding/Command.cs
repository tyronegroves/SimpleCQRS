using System;

namespace SimpleCqrs.Commanding
{
    public abstract class Command
    {
        public Guid AggregateRootId { get; set; }
    }
}