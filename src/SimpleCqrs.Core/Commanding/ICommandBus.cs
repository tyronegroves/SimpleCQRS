namespace SimpleCqrs.Commanding
{
    public interface ICommandBus
    {
        int Execute(Command command);
    }
}