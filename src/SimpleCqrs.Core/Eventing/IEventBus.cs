using System.Collections.Generic;

namespace SimpleCqrs.Eventing
{
    public interface IEventBus
    {
        void PublishEvent(DomainEvent domainEvent);
        void PublishEvents(IEnumerable<DomainEvent> domainEvents);
    }
}