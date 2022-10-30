namespace EventSourcingCQRS.Commanding
{
    internal interface ICommandDispatcher
    {
        Task<TCommandResult> Dispatch<TCommand, TCommandResult>(TCommand command, CancellationToken cancellation);
    }
}