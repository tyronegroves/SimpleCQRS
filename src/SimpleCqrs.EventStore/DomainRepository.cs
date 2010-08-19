using System;
using System.Linq;
using SimpleCqrs.Domain;
using SimpleCqrs.Events;

namespace SimpleCqrs.EventStore
{
    public class DomainRepository
    {
        private readonly IEventStore eventStore;
        private readonly IEventBus eventBus;

        public DomainRepository(IEventStore eventStore, IEventBus eventBus)
        {
            this.eventStore = eventStore;
            this.eventBus = eventBus;
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
            eventBus.PublishEvents(domainEvents);
            aggregateRoot.CommitEvents();
        }
    }
}