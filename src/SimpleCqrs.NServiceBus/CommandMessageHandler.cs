using NServiceBus;
using SimpleCqrs.Commanding;
using SimpleCqrs.NServiceBus.Commanding;

namespace SimpleCqrs.NServiceBus
{
    public class CommandMessageHandler : IHandleMessages<CommandMessage>
    {
        private readonly ICommandBus commandBus;

        public CommandMessageHandler(ICommandBus commandBus)
        {
            this.commandBus = commandBus;
        }

        public void Handle(CommandMessage message)
        {
            commandBus.Send(message.Command);
        }
    }
}