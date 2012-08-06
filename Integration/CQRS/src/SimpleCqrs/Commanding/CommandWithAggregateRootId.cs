using System;

namespace SimpleCqrs.Commanding
{
    [Serializable]
    public class CommandWithAggregateRootId : ICommandWithAggregateRootId
    {
        public Guid AggregateRootId { get; set; }
    }
}