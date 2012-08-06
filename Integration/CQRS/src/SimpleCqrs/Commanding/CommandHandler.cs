using System;

namespace SimpleCqrs.Commanding
{
    public abstract class CommandHandler<TCommand> : IHandleCommands<TCommand> where TCommand : ICommand
    {
        private ICommandHandlingContext<TCommand> context;

        void IHandleCommands<TCommand>.Handle(ICommandHandlingContext<TCommand> handlingContext)
        {
            context = handlingContext;
            Handle(handlingContext.Command);
        }

        public abstract void Handle(TCommand command);

        protected void Return(int value)
        {
            context.Return(value);
        }

        protected void Return(Enum value)
        {
            context.Return(Convert.ToInt32(value));
        }
    }
}