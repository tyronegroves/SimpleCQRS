namespace EventSourcingCQRS.Commanding
{
    public interface ICommandBus
    {
        int Execute<TCommand>(TCommand command) where TCommand : ICommand;
        void Send<TCommand>(TCommand command) where TCommand : ICommand;
    }
}