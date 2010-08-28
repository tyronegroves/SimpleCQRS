using System.Collections.Generic;

namespace SimpleCqrs.Events
{
    public interface IEventBus
    {
        void PublishEvent(DomainEvent domainEvent);
        void PublishEvents(IEnumerable<DomainEvent> domainEvents);
    }
}