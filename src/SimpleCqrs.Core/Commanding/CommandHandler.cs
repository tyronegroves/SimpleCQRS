namespace SimpleCqrs.Commanding
{
    public abstract class CommandHandler<TCommand> : IHandleCommands<TCommand> where TCommand : Command
    {
        private ICommandHandlingContext<TCommand> context;

        void IHandleCommands<TCommand>.Handle(ICommandHandlingContext<TCommand> handlingContext)
        {
            context = handlingContext;
            Handle(handlingContext.Command);
        }

        protected abstract void Handle(TCommand command);

        protected void Return(int value)
        {
            context.Return(value);
        }
    }
}