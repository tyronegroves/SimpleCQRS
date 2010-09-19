namespace SimpleCqrs.Commanding
{
    public abstract class ValidatingCommandHandler<TCommand> : IHandleCommands<TCommand> where TCommand : ICommand
    {
        void IHandleCommands<TCommand>.Handle(ICommandHandlingContext<TCommand> handlingContext)
        {
            ValidationResult = ValidateCommand(handlingContext.Command);
            handlingContext.Return(ValidationResult);
            Handle(handlingContext.Command);
        }

        protected int ValidationResult { get; private set; }

        protected virtual int ValidateCommand(TCommand command)
        {
            return 0;
        }

        protected abstract void Handle(TCommand command);
    }
}