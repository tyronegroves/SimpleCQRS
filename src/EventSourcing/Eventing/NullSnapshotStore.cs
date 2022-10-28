using System.Diagnostics.CodeAnalysis;
using EventSourcingCQRS.Domain;

namespace EventSourcingCQRS.Eventing
{
    [ExcludeFromCodeCoverage]
    public class NullSnapshotStore : ISnapshotStore
    {
        Task<Snapshot> ISnapshotStore.GetSnapshot(Guid aggregateRootId, CancellationToken cancellationToken)

        {
            return null;
        }

        Task ISnapshotStore.SaveSnapshot<TSnapshot>(TSnapshot snapshot, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}