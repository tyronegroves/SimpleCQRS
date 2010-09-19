using System;
using SimpleCqrs.Commanding;

namespace NerdDinner.Commands
{
    public class RegisterUserCommand : ICommand
    {
        public RegisterUserCommand()
        {
            UserId = Guid.NewGuid();
        }

        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}