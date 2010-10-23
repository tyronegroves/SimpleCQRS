using NServiceBus;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.NServiceBus.Eventing
{
    internal class DomainEventMessageHandler : IHandleMessages<IDomainEventMessage>
    {
        private readonly IEventBus eventBus;

        public DomainEventMessageHandler(IEventBus eventBus)
        {
            this.eventBus = eventBus;
        }

        public void Handle(IDomainEventMessage message)
        {
            var domainEvent = message.DomainEvent;
            eventBus.PublishEvent(domainEvent);
        }
    }
}