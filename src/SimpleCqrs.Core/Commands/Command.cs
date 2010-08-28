using System;

namespace SimpleCqrs.Commands
{
    public abstract class Command
    {
        public Guid AggregateRootId { get; set; }
    }
}