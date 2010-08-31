using System.ServiceModel;

namespace Commands
{
    [ServiceContract]
    public interface ICommandService
    {
        [OperationContract]
        int CreateCart(CreateCartCommand createCartCommand);

        [OperationContract]
        void AddProductToCart(AddProductToCartCommand addProductToCartCommand);
    }
}