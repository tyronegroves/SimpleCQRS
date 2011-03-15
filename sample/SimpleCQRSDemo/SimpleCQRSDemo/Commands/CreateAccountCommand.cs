using SimpleCqrs.Commanding;

namespace SimpleCQRSDemo.Commands
{
    public class CreateAccountCommand : ICommand
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}