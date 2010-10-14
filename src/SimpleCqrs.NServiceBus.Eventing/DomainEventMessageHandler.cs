using NServiceBus;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.NServiceBus.Eventing
{
    public class DomainEventMessageHandler : IHandleMessages<DomainEventMessage>
    {
        private readonly IEventBus eventBus;

        public DomainEventMessageHandler(IEventBus eventBus)
        {
            this.eventBus = eventBus;
        }

        public void Handle(DomainEventMessage message)
        {
            eventBus.PublishEvent(message.DomainEvent);
        }
    }
}