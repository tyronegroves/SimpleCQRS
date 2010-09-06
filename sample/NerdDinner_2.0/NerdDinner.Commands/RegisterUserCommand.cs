using SimpleCqrs.Commanding;

namespace NerdDinner.Commands
{
    public class RegisterUserCommand : CommandWithAggregateRootId
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}