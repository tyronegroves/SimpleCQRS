using System.ServiceModel;
using System.Web.Security;
using NerdDinner.Commands;

namespace NerdDinner.CommandService
{
    [ServiceContract]
    public interface ICommandService
    {
        [OperationContract]
        MembershipCreateStatus RegisterUser(RegisterUserCommand registerUserCommand);

        [OperationContract]
        bool ChangePassword(ChangePasswordCommand changePasswordCommand);
    }
}