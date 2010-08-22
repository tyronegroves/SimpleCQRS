using System;

namespace SimpleCqrs.Domain
{
    public interface ISnapshot
    {
        Guid AggregateRootId { get; set; }
    }
}