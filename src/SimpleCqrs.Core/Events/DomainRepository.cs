using System;
using System.Linq;
using SimpleCqrs.Domain;

namespace SimpleCqrs.Events
{
    public class DomainRepository
    {
        private readonly IEventStore eventStore;
        private readonly ISnapshotStore snapshotStore;
        private readonly IEventBus eventBus;

        public DomainRepository(IEventStore eventStore, ISnapshotStore snapshotStore, IEventBus eventBus)
        {
            this.eventStore = eventStore;
            this.snapshotStore = snapshotStore;
            this.eventBus = eventBus;
        }

        public TAggregateRoot GetById<TAggregateRoot>(Guid aggregateRootId) where TAggregateRoot : AggregateRoot, new()
        {
            var snapshot = GetSnapshotFromSnapshotStore(aggregateRootId);
            var aggregateRoot = new TAggregateRoot();
            LoadSnapshot(aggregateRoot, snapshot);

            var domainEvents = eventStore.GetEvents(aggregateRootId, snapshot.LastEventSequence);
            aggregateRoot.LoadFromHistoricalEvents(domainEvents.ToArray());

            return aggregateRoot;
        }

        public void Save(AggregateRoot aggregateRoot)
        {
            var domainEvents = aggregateRoot.UncommittedEvents;
            eventStore.Insert(domainEvents);
            eventBus.PublishEvents(domainEvents);
            aggregateRoot.CommitEvents();
        }

        private static void LoadSnapshot(AggregateRoot aggregateRoot, Snapshot snapshot)
        {
            var snapshotOriginator = aggregateRoot as ISnapshotOriginator;
            if (snapshotOriginator != null)
                snapshotOriginator.LoadSnapshot(snapshot);
        }

        private Snapshot GetSnapshotFromSnapshotStore(Guid aggregateRootId)
        {
            return snapshotStore.GetSnapshot(aggregateRootId) ?? new Snapshot();
        }
    }
}