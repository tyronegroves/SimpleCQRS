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

        public void CreateCart(CreateCartCommand createCartCommand)
        {
            commandBus.Execute(createCartCommand);
        }
    }
}