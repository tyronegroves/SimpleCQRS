using System;

namespace SimpleCqrs.Commanding
{
    public interface ICommandErrorHandler<in TCommand> where TCommand : ICommand
    {
        void Handle(TCommand command, Exception exception);
    }
}