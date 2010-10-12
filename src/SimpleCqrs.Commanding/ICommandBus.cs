namespace SimpleCqrs.Commanding
{
    public interface ICommandBus
    {
        int ExecuteWithReturnValue(ICommand command);
        void Execute(ICommand command);
    }
}