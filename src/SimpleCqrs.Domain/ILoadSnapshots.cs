namespace SimpleCqrs.Domain
{
    public interface ILoadSnapshots<TSnapshot> where TSnapshot : ISnapshot
    {
        TSnapshot GetCurrentSnapshot();
        void LoadSnapshot(TSnapshot snapshot);
    }
}