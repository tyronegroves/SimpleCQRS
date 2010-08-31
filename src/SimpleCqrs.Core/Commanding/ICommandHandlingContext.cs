namespace SimpleCqrs.Commanding
{
    public interface ICommandHandlingContext<out TCommand> where TCommand : Command
    {
        TCommand Command { get; }
        void Return(int value);
    }
}