using NServiceBus;
using SimpleCqrs.Commanding;
using SimpleCqrs.NServiceBus.Commanding;

namespace SimpleCqrs.NServiceBus
{
    public class CommandWithReturnValueMessageHandler : IHandleMessages<CommandWithReturnValueMessage>
    {
        private readonly ICommandBus commandBus;
        private readonly IBus bus;

        public CommandWithReturnValueMessageHandler(ICommandBus commandBus, IBus bus)
        {
            this.commandBus = commandBus;
            this.bus = bus;
        }

        public void Handle(CommandWithReturnValueMessage message)
        {
            var value = commandBus.Execute(message.Command);
            bus.Return(value);
        }
    }
}