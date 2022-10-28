using EventSourcingCQRS.Domain;

namespace EventSourcingCQRS.Eventing
{
    public interface ISnapshotStore
    {
        Snapshot GetSnapshot(Guid aggregateRootId);
        void SaveSnapshot<TSnapshot>(TSnapshot snapshot) where TSnapshot : Snapshot;
    }
}