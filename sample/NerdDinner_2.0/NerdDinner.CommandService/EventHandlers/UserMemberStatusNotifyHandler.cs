using NerdDinner.CommandService.Events;
using SimpleCqrs.Eventing;

namespace NerdDinner.CommandService.EventHandlers
{
    public class UserMemberStatusNotifyHandler : IHandleDomainEvents<UserMemberStatusChangedEvent>
    {
        public void Handle(UserMemberStatusChangedEvent domainEvent)
        {
            // Send email
        }
    }
}