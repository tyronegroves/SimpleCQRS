using NerdDinner.Commands;
using NerdDinner.CommandService.Domain;
using SimpleCqrs.Commanding;

namespace NerdDinner.CommandService.CommandHandlers
{
    public class RsvpForDinnerCommandHandler : AggregateRootCommandHandler<RsvpForDinnerCommand, Dinner>
    {
        protected override void Handle(RsvpForDinnerCommand command, Dinner dinner)
        {
            dinner.RegisterRsvp(command.AttendeeId, command.AttendeeName);
        }
    }
}