using Commands.Domain;
using SimpleCqrs.Commanding;

namespace Commands.CommandHandlers
{
    public class AddProductToCartCommandHandler : AggregateRootCommandHandler<AddProductToCartCommand, Cart>
    {
        protected override void Handle(AddProductToCartCommand command, Cart cart)
        {
            cart.AddProduct(command.ProductId, command.Quantity);
        }
    }
}