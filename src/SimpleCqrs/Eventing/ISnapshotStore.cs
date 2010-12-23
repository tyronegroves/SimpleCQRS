using System;
using SimpleCqrs.Domain;

namespace SimpleCqrs.Eventing
{
    public interface ISnapshotStore
    {
        Snapshot GetSnapshot(Guid aggregateRootId);
        void SaveSnapshot<TSnapshot>(TSnapshot snapshot) where TSnapshot : Snapshot;
    }
}