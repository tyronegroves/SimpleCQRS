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

    public class CreateDinnerCommandHandler : CommandHandler<CreateDinnerCommand>
    {
        private readonly IDomainRepository domainRepository;
        private readonly IUserService membershipReadModel;

        public CreateDinnerCommandHandler(IDomainRepository domainRepository, IUserService membershipReadModel)
        {
            this.domainRepository = domainRepository;
            this.membershipReadModel = membershipReadModel;
        }

        public override void Handle(CreateDinnerCommand command)
        {
            Return(ValidateCommand(command));

            var dinner = new Dinner(command.DinnerId, command.EventDate, command.Title,
                                    command.Description, command.ContactPhone, command.Host, command.Location);
            
            domainRepository.Save(dinner);
        }

        private CreateDinnerStatus ValidateCommand(CreateDinnerCommand command)
        {
            if(command.Host == null || !membershipReadModel.UserIdExists(command.Host.HostedById))
                return CreateDinnerStatus.HostUserIdDoesNotExists;

            if(command.Location == null)
                return CreateDinnerStatus.LocationNotProvided;

            return CreateDinnerStatus.Successful;
        }
    }
}