using System;
using SimpleCqrs.Domain;

namespace SimpleCqrs.EventStore
{
    public interface ISnapshotStore
    {
        TSnapshot GetSnapshot<TSnapshot>(Guid aggregateRootId) where TSnapshot : class, ISnapshot;
        void SaveSnapshot<TSnapshot>(TSnapshot snapshot) where TSnapshot : class, ISnapshot;
    }
}