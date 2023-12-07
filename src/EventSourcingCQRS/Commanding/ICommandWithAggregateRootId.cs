namespace EventSourcingCQRS.Commanding
{
    public interface ICommandWithAggregateRootId : ICommand
    {
        Guid AggregateRootId { get; }
    }
}