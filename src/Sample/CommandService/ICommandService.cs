using System.ServiceModel;

namespace Commands
{
    [ServiceContract]
    public interface ICommandService
    {
        [OperationContract(IsOneWay = true)]
        void CreateCart(CreateCartCommand createCartCommand);
    }
}