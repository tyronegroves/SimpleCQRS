using System;
using System.Linq;
using SimpleCqrs.Domain;

namespace SimpleCqrs.EventStore
{
    public class DomainRepository
    {
        private readonly IEventStore eventStore;

        public DomainRepository(IEventStore eventStore)
        {
            this.eventStore = eventStore;
        }

        public TAggregateRoot GetById<TAggregateRoot>(Guid aggregateRootId) where TAggregateRoot : AggregateRoot, new()
        {
            var domainEvents = eventStore.GetAggregateEvents(aggregateRootId);
            var aggregateRoot = new TAggregateRoot();
            aggregateRoot.ApplyEvents(domainEvents.ToArray());

            return aggregateRoot;
        }

        public void Add(AggregateRoot aggregateRoot)
        {
            var domainEvents = aggregateRoot.UncommittedEvents;
            eventStore.Insert(domainEvents.ToArray());
            aggregateRoot.CommitEvents();
        }
    }
}