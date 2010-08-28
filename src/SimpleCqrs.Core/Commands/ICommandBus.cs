namespace SimpleCqrs.Commands
{
    public interface ICommandBus
    {
        void Execute(Command command);
    }
}