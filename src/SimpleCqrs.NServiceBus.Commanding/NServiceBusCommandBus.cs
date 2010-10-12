using NServiceBus;
using SimpleCqrs.Commanding;

namespace SimpleCqrs.NServiceBus.Commanding
{
    public static class NServiceBusExtensions
    {
        public static ICallback Send(this IBus bus, ICommand command)
        {
            return bus.Send<CommandMessage>(message => message.Command = command);
        }

        public static ICallback Send(this IBus bus, string destination, ICommand command)
        {
            return bus.Send<CommandMessage>(destination, message => message.Command = command);
        }

        public static void Send(this IBus bus, string destination, string correlationId, ICommand command)
        {
            bus.Send<CommandMessage>(destination, correlationId, message => message.Command = command);
        }
    }
}