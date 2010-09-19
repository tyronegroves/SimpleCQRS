using System.Web.Security;
using NerdDinner.Commands;
using NerdDinner.CommandService.CommandHandlers;
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
            return (MembershipCreateStatus)commandBus.Execute(registerUserCommand);
        }

        public bool ChangePassword(ChangePasswordCommand changePasswordCommand)
        {
            return 0 == commandBus.Execute(changePasswordCommand);
        }

        public CreateDinnerStatus CreateDinner(CreateDinnerCommand createDinnerCommand)
        {
            return (CreateDinnerStatus)commandBus.Execute(createDinnerCommand);
        }

        public void EditDinner(EditDinnerCommand editDinnerCommand)
        {
            commandBus.Execute(editDinnerCommand);
        }

        public void RsvpForDinner(RsvpForDinnerCommand rsvpForDinnerCommand)
        {
            commandBus.Execute(rsvpForDinnerCommand);
        }

        public CancelDinnerStatus CancelDinner(CancelDinnerCommand cancelDinnerCommand)
        {
            return (CancelDinnerStatus)commandBus.Execute(cancelDinnerCommand);
        }
    }
}