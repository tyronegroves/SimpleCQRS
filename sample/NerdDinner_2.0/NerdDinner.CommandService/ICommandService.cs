using System.ServiceModel;
using System.Web.Security;
using NerdDinner.Commands;
using NerdDinner.CommandService.CommandHandlers;

namespace NerdDinner.CommandService
{
    [ServiceContract]
    public interface ICommandService
    {
        [OperationContract]
        MembershipCreateStatus RegisterUser(RegisterUserCommand registerUserCommand);

        [OperationContract]
        bool ChangePassword(ChangePasswordCommand changePasswordCommand);

        [OperationContract]
        CreateDinnerStatus CreateDinner(CreateDinnerCommand createDinnerCommand);

        [OperationContract]
        CancelDinnerStatus CancelDinner(CancelDinnerCommand cancelDinnerCommand);

        [OperationContract(IsOneWay = true)]
        void EditDinner(EditDinnerCommand editDinnerCommand);

        [OperationContract(IsOneWay = true)]
        void RsvpForDinner(RsvpForDinnerCommand rsvpForDinnerCommand);
    }
}