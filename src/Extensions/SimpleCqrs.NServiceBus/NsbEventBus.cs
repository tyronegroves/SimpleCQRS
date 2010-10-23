using System;
using System.Collections.Generic;
using System.Linq;
using NServiceBus;
using SimpleCqrs.Eventing;
using SimpleCqrs.NServiceBus.Eventing;

namespace SimpleCqrs.NServiceBus
{
    internal class NsbEventBus : IEventBus
    {
        private IBus bus;

        public void PublishEvent(DomainEvent domainEvent)
        {
            Bus.Publish<IDomainEventMessage>(message => message.DomainEvent = domainEvent);
        }

        public void PublishEvents(IEnumerable<DomainEvent> domainEvents)
        {
            var domainEventMessages = domainEvents.Select(CreateDomainEventMessage).ToArray();
            Bus.Publish(domainEventMessages);
        }

        private IBus Bus
        {
            get { return bus ?? (bus = Configure.Instance.Builder.Build<IBus>()); }
        }

        private static IDomainEventMessage CreateDomainEventMessage(DomainEvent domainEvent)
        {
            var domainEventMessageType = typeof(DomainEventMessage<>).MakeGenericType(domainEvent.GetType());
            var message = (IDomainEventMessage)Activator.CreateInstance(domainEventMessageType);
            message.DomainEvent = domainEvent;
            return message;
        }
    }
}