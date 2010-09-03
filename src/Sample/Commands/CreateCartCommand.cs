using System;
using SimpleCqrs.Commanding;

namespace Commands
{
    public class CreateCartCommand : ICommand
    {
        public CreateCartCommand()
        {
            CartId = Guid.NewGuid();
        }

        public Guid CartId { get; private set; }
    }
}