using SimpleCqrs.Commanding;

namespace Commands.CommandHandlers
{
    public class AddProductToCartCommandHandler : IHandleCommands<AddProductToCartCommand>
    {
        public void Handle(ICommandHandlingContext<AddProductToCartCommand> handlingContext)
        {
        }
    }
}