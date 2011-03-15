namespace SimpleCqrs.Domain
{
    public interface IGenerateEventSequences
    {
        int GetNextSequence();
    }
}