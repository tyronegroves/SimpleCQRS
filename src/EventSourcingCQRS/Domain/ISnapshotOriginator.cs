namespace EventSourcingCQRS.Domain
{
    public interface ISnapshotOriginator 
    {
        Snapshot GetSnapshot();
        void LoadSnapshot(Snapshot snapshot);
        bool ShouldTakeSnapshot(Snapshot previousSnapshot);
    }
}