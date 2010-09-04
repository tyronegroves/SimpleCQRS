using System;
using SimpleCqrs.Domain;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.EventStore.File
{
    public class FileSnapshotStore : ISnapshotStore
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