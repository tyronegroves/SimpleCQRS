using NerdDinner.Commands;
using NerdDinner.CommandService.Domain;
using SimpleCqrs.Commanding;

namespace NerdDinner.CommandService.CommandHandlers
{
    public class EditDinnerCommandHandler : AggregateRootCommandHandler<EditDinnerCommand, Dinner>
    {
        public override void Handle(EditDinnerCommand command, Dinner dinner)
        {
            dinner.Edit(command.EventDate, command.Title, command.Description, command.ContactPhone, command.Host, command.Location);
        }
    }
}