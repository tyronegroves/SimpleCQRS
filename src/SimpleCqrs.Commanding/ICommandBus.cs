namespace SimpleCqrs.Commanding
{
    public interface ICommandBus
    {
        int Execute(ICommand command);
        void Send(ICommand command);
    }
}