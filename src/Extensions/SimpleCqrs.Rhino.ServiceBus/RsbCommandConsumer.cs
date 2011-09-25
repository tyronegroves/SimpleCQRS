using System;
using Rhino.ServiceBus;
using SimpleCqrs.Commanding;

namespace SimpleCqrs.Rhino.ServiceBus
{
    public class RsbCommandConsumer : ConsumerOf<ICommand>, ConsumerOf<IRequest>
    {
        private readonly ICommandBus commandBus;
        private readonly IServiceBus serviceBus;

        public RsbCommandConsumer(ICommandBus commandBus, IServiceBus serviceBus)
        {
            this.commandBus = commandBus;
            this.serviceBus = serviceBus;
        }

        public void Consume(ICommand command)
        {
            commandBus.Send(command);
        }

        public void Consume(IRequest request)
        {
            var validationResult = commandBus.Execute(request.Command);
            var reply = (IReply)Activator.CreateInstance(typeof(Reply<>).MakeGenericType(request.Command.GetType()));
            reply.ValidationResult = validationResult;
            serviceBus.Reply(reply);
        }
    }
}