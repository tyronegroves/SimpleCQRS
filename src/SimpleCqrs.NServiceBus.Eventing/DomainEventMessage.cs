using System;
using NServiceBus;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.NServiceBus.Eventing
{
    [Serializable]
    public class DomainEventMessage : IMessage
    {
        public DomainEvent DomainEvent { get; set; }
    }
}