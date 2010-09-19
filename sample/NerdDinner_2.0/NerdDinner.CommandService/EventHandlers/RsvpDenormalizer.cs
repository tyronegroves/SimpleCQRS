using NerdDinner.CommandService.Events;
using NerdDinner.CommandService.Models;
using SimpleCqrs.Eventing;

namespace NerdDinner.CommandService.EventHandlers
{
    public class RsvpDenormalizer : IHandleDomainEvents<DinnerRsvpAddedEvent>
    {
        private readonly NerdDinnerEntities db = new NerdDinnerEntities();

        public void Handle(DinnerRsvpAddedEvent domainEvent)
        {
            var rsvp = RsvpReadModel.CreateRsvpReadModel(domainEvent.DinnerId, domainEvent.AttendeeId, domainEvent.AttendeeName);
            db.AddToRsvpReadModels(rsvp);
            db.SaveChanges();
        }
    }
}