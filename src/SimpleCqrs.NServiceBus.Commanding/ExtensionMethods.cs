using NServiceBus;
using SimpleCqrs.Commanding;

namespace SimpleCqrs.NServiceBus.Commanding
{
    public static class ExtensionMethods
    {
        public static ICallback ExecuteWithCallback(this ICommandBus commandBus, ICommand command)
        {
            var bus = (CommandBus)commandBus;
            return bus.InnerBus.Send<CommandWithReturnValueMessage>(message => message.Command = command);
        }

        public static int ExecuteWeb(this ICommandBus commandBus, ICommand command)
        {
            var bus = (CommandBus)commandBus;
            var returnValue = 0;
            bus.InnerBus.Send<CommandWithReturnValueMessage>(message => message.Command = command)
                .RegisterWebCallback(errorCode => returnValue = errorCode, null);

            return returnValue;
        }
    }
}