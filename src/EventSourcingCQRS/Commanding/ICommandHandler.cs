namespace EventSourcingCQRS.Commanding
{
    internal interface ICommandHandler<in TCommand, TCommandResult>
    {
        Task<TCommandResult> Handle(TCommand command, CancellationToken cancellation);
    }
}