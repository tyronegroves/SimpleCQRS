using Rhino.ServiceBus;
using SimpleCqrs.Commanding;

namespace SimpleCqrs.Rhino.ServiceBus
{
    public class RsbCommandBus : ICommandBus
    {
        private readonly IServiceBus serviceBus;

        public RsbCommandBus(IServiceBus serviceBus)
        {
            this.serviceBus = serviceBus;
        }

        public int Execute<TCommand>(TCommand command) where TCommand : ICommand
        {
            var replyConsumer = new ReplyConsumer<TCommand>();
            using(serviceBus.AddInstanceSubscription(replyConsumer))
            {
                serviceBus.Send(new Request<TCommand>{Command = command });
                if (!replyConsumer.Event.WaitOne(50000))
                    throw new ExecuteTimeoutException();

                return replyConsumer.ValidationResult;
            }
        }

        public void Send<TCommand>(TCommand command) where TCommand : ICommand
        {
            serviceBus.Send(command);
        }
    }
}