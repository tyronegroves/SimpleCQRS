using System;
using System.Collections.Generic;
using System.Linq;
using NServiceBus;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.NServiceBus.Eventing
{
    public class NsbEventBus : IEventBus
    {
        private IBus bus;

        public void PublishEvent(DomainEvent domainEvent)
        {
            Bus.Publish<IDomainEventMessage>(message => message.DomainEvent = domainEvent);
        }

        public void PublishEvents(IEnumerable<DomainEvent> domainEvents)
        {
            var domainEventMessages = domainEvents.Select(CreateDomainEventMessage).ToList();
            domainEventMessages.ForEach(message => Bus.Publish(message));
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