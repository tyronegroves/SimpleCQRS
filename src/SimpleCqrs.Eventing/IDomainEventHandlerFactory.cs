using System;

namespace SimpleCqrs.Eventing
{
    public interface IDomainEventHandlerFactory
    {
        object Create(Type domainEventHandlerType);
    }
}