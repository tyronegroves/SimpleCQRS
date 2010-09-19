using NerdDinner.Commands;
using NerdDinner.CommandService.Domain;
using NerdDinner.CommandService.Models;
using SimpleCqrs.Commanding;
using SimpleCqrs.Domain;

namespace NerdDinner.CommandService.CommandHandlers
{
    public enum CreateDinnerStatus
    {
        HostUserIdDoesNotExists,
        Successful,
        LocationNotProvided,
    }

    public class CreateDinnerCommandHandler : ValidatingCommandHandler<CreateDinnerCommand>
    {
        private readonly IDomainRepository domainRepository;
        private readonly IUserService membershipReadModel;

        public CreateDinnerCommandHandler(IDomainRepository domainRepository, IUserService membershipReadModel)
        {
            this.domainRepository = domainRepository;
            this.membershipReadModel = membershipReadModel;
        }

        protected override int ValidateCommand(CreateDinnerCommand command)
        {
            if(command.Host == null || !membershipReadModel.UserIdExists(command.Host.HostedById))
                return (int)CreateDinnerStatus.HostUserIdDoesNotExists;

            if (command.Location == null)
                return (int)CreateDinnerStatus.LocationNotProvided;

            return (int)CreateDinnerStatus.Successful;
        }

        protected override void Handle(CreateDinnerCommand command)
        {
            var dinner = new Dinner(command.DinnerId, command.EventDate, command.Title,
                                    command.Description, command.ContactPhone, command.Host, command.Location);

            domainRepository.Save(dinner);
        }
    }
}