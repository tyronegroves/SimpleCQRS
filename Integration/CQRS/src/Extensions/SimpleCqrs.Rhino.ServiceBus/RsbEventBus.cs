using System.Collections.Generic;
using System.Linq;
using Rhino.ServiceBus;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.Rhino.ServiceBus
{
    public class RsbEventBus : IEventBus
    {
        private readonly IServiceBus serviceBus;

        public RsbEventBus(IServiceBus serviceBus)
        {
            this.serviceBus = serviceBus;
        }

        public void PublishEvent(DomainEvent domainEvent)
        {
            serviceBus.Notify(domainEvent);
        }

        public void PublishEvents(IEnumerable<DomainEvent> domainEvents)
        {
            serviceBus.Notify(domainEvents.ToArray());
        }
    }
}