namespace SimpleCqrs.Commands
{
    public interface IHandleCommands<TCommand> where TCommand : Command
    {
        void Handle(TCommand command);
    }
}