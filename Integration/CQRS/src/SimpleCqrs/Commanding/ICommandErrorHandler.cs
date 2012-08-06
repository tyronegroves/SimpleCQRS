using System;

namespace SimpleCqrs.Commanding
{
    public interface ICommandErrorHandler<in TCommand> where TCommand : ICommand
    {
        void Handle(ICommandHandlingContext<TCommand> handlingContext, Exception exception);
    }
}