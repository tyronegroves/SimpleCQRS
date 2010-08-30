using SimpleCqrs.Commanding;

namespace Commands.CommandHandlers
{
    public class Cart : IHandleCommands<CreateCartCommand>
    {
        public int Handle(CreateCartCommand command)
        {
            return 0;
        }
    }
}