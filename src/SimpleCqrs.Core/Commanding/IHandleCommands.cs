namespace SimpleCqrs.Commanding
{
    public interface IHandleCommands<in TCommand> where TCommand : ICommand
    {
        void Handle(ICommandHandlingContext<TCommand> handlingContext);
    }
}