using System;

namespace SimpleCqrs.Commanding
{
    public class CommandWithAggregateRootId : ICommandWithAggregateRootId
    {
        public Guid AggregateRootId { get; set; }
    }
}