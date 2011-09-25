using Rhino.ServiceBus;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.Rhino.ServiceBus
{
    public class DomainEventConsumer : ConsumerOf<DomainEvent>
    {
        private readonly IEventBus eventBus;

        public DomainEventConsumer(IEventBus eventBus)
        {
            this.eventBus = eventBus;
        }

        public void Consume(DomainEvent domainEvent)
        {
            eventBus.PublishEvent(domainEvent);
        }
    }
}