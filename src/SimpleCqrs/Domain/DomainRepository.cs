using System;
using System.Linq;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.Domain
{
    public class DomainRepository : IDomainRepository
    {
        private readonly IEventBus eventBus;
        private readonly IEventStore eventStore;
        private readonly ISnapshotStore snapshotStore;

        public DomainRepository(IEventStore eventStore, ISnapshotStore snapshotStore, IEventBus eventBus)
        {
            this.eventStore = eventStore;
            this.snapshotStore = snapshotStore;
            this.eventBus = eventBus;
        }

        public virtual TAggregateRoot GetById<TAggregateRoot>(Guid aggregateRootId) where TAggregateRoot : AggregateRoot, new()
        {
            var aggregateRoot = new TAggregateRoot();
            var snapshot = GetSnapshotFromSnapshotStore(aggregateRootId);
            var lastEventSequence = snapshot == null || !(aggregateRoot is ISnapshotOriginator) ? 0 : snapshot.LastEventSequence;
            var domainEvents = eventStore.GetEvents(aggregateRootId, lastEventSequence);

            if (lastEventSequence == 0 && domainEvents.Count() == 0)
                return null;


            LoadSnapshot(aggregateRoot, snapshot);
            aggregateRoot.LoadFromHistoricalEvents(domainEvents.ToArray());

            return aggregateRoot;
        }

        public virtual void Save(AggregateRoot aggregateRoot)
        {
            var domainEvents = aggregateRoot.UncommittedEvents;

            eventStore.Insert(domainEvents);
            eventBus.PublishEvents(domainEvents);
            
            aggregateRoot.CommitEvents();

            SaveSnapshot(aggregateRoot);
        }

        private void SaveSnapshot(AggregateRoot aggregateRoot)
        {
            var snapshotOriginator = aggregateRoot as ISnapshotOriginator;
            
            if(snapshotOriginator == null)
                return;

            var previousSnapshot = snapshotStore.GetSnapshot(aggregateRoot.Id);

            if (!snapshotOriginator.ShouldTakeSnapshot(previousSnapshot)) return;

            var snapshot = snapshotOriginator.GetSnapshot();
            snapshot.AggregateRootId = aggregateRoot.Id;
            snapshot.LastEventSequence = aggregateRoot.LastEventSequence;

            snapshotStore.SaveSnapshot(snapshot);
        }

        private static void LoadSnapshot(AggregateRoot aggregateRoot, Snapshot snapshot)
        {
            var snapshotOriginator = aggregateRoot as ISnapshotOriginator;
            if(snapshot != null && snapshotOriginator != null)
            {
                snapshotOriginator.LoadSnapshot(snapshot);
                aggregateRoot.Id = snapshot.AggregateRootId;
                aggregateRoot.LastEventSequence = snapshot.LastEventSequence;
            }
        }

        private Snapshot GetSnapshotFromSnapshotStore(Guid aggregateRootId)
        {
            return snapshotStore.GetSnapshot(aggregateRootId);
        }
    }
}