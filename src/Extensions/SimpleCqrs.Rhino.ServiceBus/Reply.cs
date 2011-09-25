using SimpleCqrs.Commanding;

namespace SimpleCqrs.Rhino.ServiceBus
{
    public class Reply<TCommand> : IReply where TCommand : ICommand
    {
        public int ValidationResult { get; set; }
    }

    public interface IReply
    {
        int ValidationResult { get; set; }
    }
}