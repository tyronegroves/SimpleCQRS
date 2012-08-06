using System;
using SimpleCqrs.Domain;

namespace SimpleCqrs.Eventing
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