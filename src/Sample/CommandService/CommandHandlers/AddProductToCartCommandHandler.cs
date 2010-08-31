using Commands.Domain;
using SimpleCqrs.Commanding;
using SimpleCqrs.Eventing;

namespace Commands.CommandHandlers
{
    public class AddProductToCartCommandHandler : CommandHandler<AddProductToCartCommand>
    {
        private readonly IDomainRepository domainRepository;

        public AddProductToCartCommandHandler(IDomainRepository domainRepository)
        {
            this.domainRepository = domainRepository;
        }

        protected override void Handle(AddProductToCartCommand command)
        {
            Return(0);

            var cart = domainRepository.GetById<Cart>(command.Id);
            cart.AddProduct(command.ProductId, command.Quantity);

            domainRepository.Save(cart);
        }
    }
}