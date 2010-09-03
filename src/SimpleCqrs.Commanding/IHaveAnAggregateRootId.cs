using System;

namespace SimpleCqrs.Commanding
{
    public interface ICommandWithAggregateRootId : ICommand
    {
        Guid AggregateRootId { get; }
    }
}