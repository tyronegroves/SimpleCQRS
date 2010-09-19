using NerdDinner.Commands;
using NerdDinner.CommandService.Domain;
using SimpleCqrs.Commanding;

namespace NerdDinner.CommandService.CommandHandlers
{
    public class ChangePasswordCommandHandler : AggregateRootCommandHandler<ChangePasswordCommand, User>
    {
        protected override void Handle(ChangePasswordCommand command, User user)
        {
            user.ChangePassword(command.NewPassword);
        }
    }
}