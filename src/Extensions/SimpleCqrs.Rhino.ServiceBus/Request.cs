using SimpleCqrs.Commanding;

namespace SimpleCqrs.Rhino.ServiceBus
{
    public class Request<TCommand> : IRequest where TCommand : ICommand
    {
        public TCommand Command { get; set; }

        ICommand IRequest.Command
        {
            get { return Command; }
            set { Command = (TCommand)value; }
        }
    }

    public interface IRequest
    {
        ICommand Command { get; set; }
    }
}