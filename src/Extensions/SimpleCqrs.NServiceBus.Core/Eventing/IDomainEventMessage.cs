using NServiceBus;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.NServiceBus.Eventing
{
    public interface IDomainEventMessage : IMessage
    {
        DomainEvent DomainEvent { get; set; }
        string Header { get; set; }
    }
}