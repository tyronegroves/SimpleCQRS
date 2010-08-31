namespace SimpleCqrs.Commanding
{
    public interface IHandleCommands<in TCommand> where TCommand : Command
    {
        void Handle(ICommandHandlingContext<TCommand> handlingContext);
    }
}