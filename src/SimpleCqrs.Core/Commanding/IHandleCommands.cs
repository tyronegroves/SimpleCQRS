namespace SimpleCqrs.Commanding
{
    public interface IHandleCommands<in TCommand> where TCommand : Command
    {
        int Handle(TCommand command);
    }
}