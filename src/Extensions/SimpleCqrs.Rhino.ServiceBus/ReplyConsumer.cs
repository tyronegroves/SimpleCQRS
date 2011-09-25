using System;
using System.Threading;
using Rhino.ServiceBus;
using SimpleCqrs.Commanding;

namespace SimpleCqrs.Rhino.ServiceBus
{
    public class ReplyConsumer<TCommand> : OccasionalConsumerOf<Reply<TCommand>> where TCommand : ICommand
    {
        public ReplyConsumer()
        {
            Event = new ManualResetEvent(false);
        }

        public ManualResetEvent Event { get; set; }
        public int ValidationResult { get; set; }

        public void Consume(Reply<TCommand> replay)
        {
            ValidationResult = replay.ValidationResult;
            Event.Set();
        }
    }
}