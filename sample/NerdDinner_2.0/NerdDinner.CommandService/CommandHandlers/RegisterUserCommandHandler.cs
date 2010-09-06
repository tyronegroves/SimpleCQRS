using System.Web.Security;
using NerdDinner.Commands;
using NerdDinner.CommandService.Domain;
using SimpleCqrs.Commanding;
using SimpleCqrs.Eventing;

namespace NerdDinner.CommandService.CommandHandlers
{
    public class RegisterUserCommandHandler : CommandHandler<RegisterUserCommand>
    {
        private readonly IDomainRepository domainRepository;

        public RegisterUserCommandHandler(IDomainRepository domainRepository)
        {
            this.domainRepository = domainRepository;
        }

        protected override void Handle(RegisterUserCommand command)
        {
            Return((int) MembershipCreateStatus.Success);
            var user = new User(command.AggregateRootId);
            domainRepository.Save(user);
        }
    }
}