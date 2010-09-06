using System.Web.Security;
using NerdDinner.Commands;
using SimpleCqrs;
using SimpleCqrs.Commanding;

namespace NerdDinner.CommandService
{
    public class CommandService : ICommandService
    {
        private readonly ICommandBus commandBus;

        public CommandService()
            : this(ServiceLocator.Current.Resolve<ICommandBus>())
        {
        }

        public CommandService(ICommandBus commandBus)
        {
            this.commandBus = commandBus;
        }

        public MembershipCreateStatus RegisterUser(RegisterUserCommand registerUserCommand)
        {
            return (MembershipCreateStatus) commandBus.Execute(registerUserCommand);
        }

        public bool ChangePassword(ChangePasswordCommand changePasswordCommand)
        {
            return 0 == commandBus.Execute(changePasswordCommand);
        }
    }
}