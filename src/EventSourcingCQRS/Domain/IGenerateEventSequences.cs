namespace EventSourcingCQRS.Domain
{
    public interface IGenerateEventSequences
    {
        int GetNextSequence();
    }
}