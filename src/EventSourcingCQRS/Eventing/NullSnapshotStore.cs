using EventSourcingCQRS.Domain;

namespace EventSourcingCQRS.Eventing
{
    public class NullSnapshotStore : ISnapshotStore
    {
        public Snapshot GetSnapshot(Guid aggregateRootId)
        {
            return null;
        }

        public void SaveSnapshot<TSnapshot>(TSnapshot snapshot) where TSnapshot : Snapshot
        {
        }
    }
}