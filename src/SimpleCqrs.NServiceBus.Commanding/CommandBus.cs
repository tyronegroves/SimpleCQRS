using NServiceBus;
using SimpleCqrs.Commanding;

namespace SimpleCqrs.NServiceBus.Commanding
{
    public class CommandBus : ICommandBus
    {
        private IBus bus;

        public IBus InnerBus
        {
            get { return bus ?? (bus = Configure.Instance.Builder.Build<IBus>()); }
        }

        public int Execute(ICommand command)
        {
            var returnValue = 0;
            
            InnerBus.Send<CommandWithReturnValueMessage>(message => message.Command = command)
                .Register(errorCode => returnValue = errorCode);

            return returnValue;
        }

        public void Send(ICommand command)
        {
            InnerBus.Send<CommandMessage>(message => message.Command = command);
        }
    }
}