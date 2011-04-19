using NerdDinner.Commands;
using NerdDinner.CommandService.Domain;
using SimpleCqrs.Commanding;
using SimpleCqrs.Domain;

namespace NerdDinner.CommandService.CommandHandlers
{
    public class RegisterUserCommandHandler : CommandHandler<RegisterUserCommand>
    {
        private readonly IDomainRepository domainRepository;

        public RegisterUserCommandHandler(IDomainRepository domainRepository)
        {
            this.domainRepository = domainRepository;
        }

        public override void Handle(RegisterUserCommand command)
        {
            var user = new User(command.UserId, command.UserName, command.Password, command.Email);
            domainRepository.Save(user);
        }
    }
}