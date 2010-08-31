using System.ServiceModel;
using SimpleCqrs.Commanding;

namespace Commands
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class CommandService : ICommandService
    {
        private readonly ICommandBus commandBus;

        public CommandService(ICommandBus commandBus)
        {
            this.commandBus = commandBus;
        }

        public int CreateCart(CreateCartCommand createCartCommand)
        {
            return commandBus.Execute(createCartCommand);
        }

        public void AddProductToCart(AddProductToCartCommand addProductToCartCommand)
        {
            commandBus.Execute(addProductToCartCommand);
        }
    }
}