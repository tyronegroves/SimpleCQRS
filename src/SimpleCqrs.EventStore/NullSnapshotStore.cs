using System;
using SimpleCqrs.Domain;

namespace SimpleCqrs.EventStore
{
    public class NullSnapshotStore : ISnapshotStore
    {
        public TSnapshot GetSnapshot<TSnapshot>(Guid aggregateRootId) where TSnapshot : class, ISnapshot
        {
            return null;
        }

        public void SaveSnapshot<TSnapshot>(TSnapshot snapshot) where TSnapshot : class, ISnapshot
        {
        }
    }
}