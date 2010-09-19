namespace SimpleCqrs.Commanding
{
    public abstract class CommandHandler<TCommand> : IHandleCommands<TCommand> where TCommand : ICommand
    {
        void IHandleCommands<TCommand>.Handle(ICommandHandlingContext<TCommand> handlingContext)
        {
            handlingContext.Return(0);
            Handle(handlingContext.Command);
        }

        protected abstract void Handle(TCommand command);
    }
}