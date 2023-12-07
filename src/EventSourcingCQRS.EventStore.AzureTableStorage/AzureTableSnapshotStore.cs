using EventSourcingCQRS.Domain;
using EventSourcingCQRS.Eventing;

namespace EventSourcingCQRS.EventStore.AzureTableStorage
{
    public class AzureTableSnapshotStore : ISnapshotStore
    {
        public Task<Snapshot> GetSnapshot(Guid aggregateRootId, CancellationToken cancellationToken)
        {
            Snapshot documents = null;
            return Task.FromResult(documents) ;

        }

        public Task SaveSnapshot<TSnapshot>(TSnapshot snapshot, CancellationToken cancellationToken) where TSnapshot : Snapshot
        {
            throw new NotImplementedException();
        }
    }
}
