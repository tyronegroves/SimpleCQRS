using System.Collections.Generic;

namespace SimpleCqrs.Events
{
    public interface IEventBus
    {
        void PublishEvent(IDomainEvent domainEvent);
        void PublishEvents(IEnumerable<IDomainEvent> domainEvents);
    }
}