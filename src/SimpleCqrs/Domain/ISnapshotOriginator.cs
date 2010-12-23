namespace SimpleCqrs.Domain
{
    public interface ISnapshotOriginator 
    {
        Snapshot GetSnapshot();
        void LoadSnapshot(Snapshot snapshot);
        bool HasSnapshot { get; }
    }
}