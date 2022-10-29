using System.Collections.ObjectModel;
using EventSourcingCQRS.Eventing;

namespace EventSourcingCQRS.Domain
{
    public class DomainRepository : IDomainRepository
    {
        private readonly IEventBus _eventBus;
        private readonly IEventStore _eventStore;
        private readonly ISnapshotStore _snapshotStore;

        public DomainRepository(IEventStore eventStore, ISnapshotStore snapshotStore, IEventBus eventBus)
        {
            _eventStore = eventStore;
            _snapshotStore = snapshotStore;
            _eventBus = eventBus;
        }

        public virtual async Task<TAggregateRoot> GetById<TAggregateRoot>(Guid aggregateRootId,
            CancellationToken cancellationToken) where TAggregateRoot : AggregateRoot, new()
        {
            var aggregateRoot = new TAggregateRoot();
            var snapshot = await GetSnapshotFromSnapshotStore(aggregateRootId, cancellationToken);
            var lastEventSequence = snapshot == null || (aggregateRoot is not ISnapshotOriginator)
                ? 0
                : snapshot.LastEventSequence;
            var domainEventsAsync = await _eventStore.GetEvents(aggregateRootId, lastEventSequence);
            var domainEvents = domainEventsAsync.ToArray();

            if (lastEventSequence == 0 && domainEvents.Length == 0)
            {
                return null;
            }

            LoadSnapshot(aggregateRoot, snapshot);
            aggregateRoot.LoadFromHistoricalEvents(domainEvents);

            return aggregateRoot;
        }

        public virtual async Task<TAggregateRoot> GetExistingById<TAggregateRoot>(Guid aggregateRootId,
            CancellationToken cancellationToken) where TAggregateRoot : AggregateRoot, new()
        {
            var aggregateRoot = await GetById<TAggregateRoot>(aggregateRootId, cancellationToken);

            if (aggregateRoot == null)
            {
                throw new AggregateRootNotFoundException(aggregateRootId, typeof(TAggregateRoot));
            }

            return aggregateRoot;
        }

        public async Task<TAggregateRoot> GetExistingByIdUpToSequence<TAggregateRoot>(Guid aggregateRootId,
            int sequence,
            CancellationToken cancellationToken) where TAggregateRoot : AggregateRoot, new()
        {
            var aggregateRoot = new TAggregateRoot();
            var snapshot = await GetSnapshotFromSnapshotStore(aggregateRootId, cancellationToken);

            var lastEventSequence = sequence;

            var domainEventsAsync = await _eventStore.GetEventsUpToSequence(aggregateRootId, lastEventSequence);
            var domainEvents = domainEventsAsync.ToArray();

            if (lastEventSequence == 0 && domainEvents.Length == 0)
            {
                return null;
            }

            aggregateRoot.LoadFromHistoricalEvents(domainEvents);

            return aggregateRoot;
        }

        public virtual async Task Save(AggregateRoot aggregateRoot, CancellationToken cancellationToken)
        {
            if (aggregateRoot.UncommittedEvents.Count == 0)
            {
                return;
            }

            var domainEvents = aggregateRoot.UncommittedEvents;

            await _eventStore.Insert(domainEvents);
            await _eventBus.PublishEvents(domainEvents, cancellationToken);

            aggregateRoot.CommitEvents();

            await SaveSnapshot(aggregateRoot, domainEvents, cancellationToken);
        }

        private async Task SaveSnapshot(AggregateRoot aggregateRoot, ReadOnlyCollection<DomainEvent> domainEvents, CancellationToken cancellationToken)
        {
            if (aggregateRoot is not ISnapshotOriginator snapshotOriginator)
            {
                return;
            }

            var previousSnapshot = await _snapshotStore.GetSnapshot(aggregateRoot.Id, cancellationToken);

            if (!snapshotOriginator.ShouldTakeSnapshot(previousSnapshot, domainEvents))
            {
                return;
            }

            var snapshot = snapshotOriginator.GetSnapshot();
            snapshot.AggregateRootId = aggregateRoot.Id;
            snapshot.LastEventSequence = aggregateRoot.LastEventSequence;

            await _snapshotStore.SaveSnapshot(snapshot, cancellationToken);
        }

        private static void LoadSnapshot(AggregateRoot aggregateRoot, Snapshot snapshot)
        {
            if (snapshot == null || !(aggregateRoot is ISnapshotOriginator snapshotOriginator))
            {
                return;
            }

            snapshotOriginator.LoadSnapshot(snapshot);
            aggregateRoot.Id = snapshot.AggregateRootId;
            aggregateRoot.LastEventSequence = snapshot.LastEventSequence;
        }

        public async Task<Snapshot> GetSnapshotFromSnapshotStore(Guid aggregateRootId, CancellationToken cancellationToken)
        {
            return await _snapshotStore.GetSnapshot(aggregateRootId, cancellationToken);
        }

        public async Task SaveSnapshotToSnapshotStore(Snapshot snapshot, CancellationToken cancellationToken)
        {
            await _snapshotStore.SaveSnapshot(snapshot, cancellationToken);
        }
    }
}