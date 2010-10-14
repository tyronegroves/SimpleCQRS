using System.Collections.Generic;
using System.Linq;
using NServiceBus;
using SimpleCqrs.Eventing;
using SimpleCqrs.NServiceBus.Eventing;

namespace SimpleCqrs.NServiceBus
{
    internal class EventBus : IEventBus
    {
        private IBus bus;

        public void PublishEvent(DomainEvent domainEvent)
        {
            Bus.Publish<DomainEventMessage>(message => message.DomainEvent = domainEvent);
        }

        public void PublishEvents(IEnumerable<DomainEvent> domainEvents)
        {
            var domainEventMessages = domainEvents.Select(e => new DomainEventMessage {DomainEvent = e}).ToArray();
            Bus.Publish(domainEventMessages);
        }

        private IBus Bus
        {
            get { return bus ?? (bus = Configure.Instance.Builder.Build<IBus>()); }
        }
    }
}