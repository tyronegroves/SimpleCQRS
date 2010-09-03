namespace SimpleCqrs.Commanding
{
    public interface ICommandHandlingContext<out TCommand> where TCommand : ICommand
    {
        TCommand Command { get; }
        void Return(int value);
    }
}