using System;
using NerdDinner.Commands;
using SimpleCqrs.Commanding;

namespace NerdDinner.CommandService.CommandHandlers
{
    public class RegisterUserCommandErrorHandler : ICommandErrorHandler<RegisterUserCommand>
    {
        public void Handle(ICommandHandlingContext<RegisterUserCommand> handlingContext, Exception exception)
        {
        }
    }
}