namespace SimpleCqrs.Commands
{
    public interface ICommandBus
    {
        int Execute(Command command);
    }
}