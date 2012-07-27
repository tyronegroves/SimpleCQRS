using System;
using SimpleCqrs.Domain;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.EventStore.AzureBlob
{
    public class AzureBlobSnapshotStore : ISnapshotStore
    {
        public Snapshot GetSnapshot(Guid aggregateRootId)
        {
            throw new NotImplementedException();
        }

        public void SaveSnapshot<TSnapshot>(TSnapshot snapshot) where TSnapshot : Snapshot
        {
            throw new NotImplementedException();
        }
    }
}