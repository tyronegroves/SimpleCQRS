using System.Collections.ObjectModel;
using EventSourcingCQRS.Eventing;

namespace EventSourcingCQRS.Domain
{
    public interface ISnapshotOriginator
    {
        Snapshot GetSnapshot();

        void LoadSnapshot(Snapshot snapshot);

        bool ShouldTakeSnapshot(Snapshot previousSnapshot, ReadOnlyCollection<DomainEvent> domainEvents);
    }
}