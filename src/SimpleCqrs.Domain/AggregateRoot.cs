using System.Collections.Generic;
using System.Linq;
using SimpleCqrs.Events;

namespace SimpleCqrs.Domain
{
    public abstract class AggregateRoot
    {
        private readonly IAggregateRootEventHandlerInvoker aggregateRootEventHandlerInvoker;

        protected AggregateRoot(IAggregateRootEventHandlerInvoker aggregateRootEventHandlerInvoker)
        {
            this.aggregateRootEventHandlerInvoker = aggregateRootEventHandlerInvoker;
        }

        public void RestoreFromEvents(IEnumerable<IDomainEvent> domainEvents)
        {
            domainEvents.ToList()
                .ForEach(e => aggregateRootEventHandlerInvoker.Invoke(this, e));
        }

        protected void ApplyEvent(IDomainEvent domainEvent)
        {
        }
    }
}