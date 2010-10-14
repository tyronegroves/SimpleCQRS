using System;
using NServiceBus;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.NServiceBus.Eventing
{
    internal class DomainEventHandlerFactory : IDomainEventHandlerFactory
    {
        public object Create(Type domainEventHandlerType)
        {
            return Configure.Instance.Builder.Build(domainEventHandlerType);
        }
    }
}