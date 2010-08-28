using System.Collections.Generic;

namespace SimpleCqrs.Events
{
    public class DirectEventBus : IEventBus
    {
        private readonly ITypeCatalog typeCatalog;

        public DirectEventBus(ITypeCatalog typeCatalog)
        {
            this.typeCatalog = typeCatalog;
        }

        public void PublishEvent(DomainEvent domainEvent)
        {

        }

        public void PublishEvents(IEnumerable<DomainEvent> domainEvents)
        {
        }
    }
}