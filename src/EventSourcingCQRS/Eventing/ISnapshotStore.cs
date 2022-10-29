using EventSourcingCQRS.Domain;

namespace EventSourcingCQRS.Eventing
{
    public interface ISnapshotStore
    {
        Task<Snapshot> GetSnapshot(Guid aggregateRootId, CancellationToken cancellationToken);

        Task SaveSnapshot<TSnapshot>(TSnapshot snapshot, CancellationToken cancellationToken) where TSnapshot : Snapshot;
    }
}