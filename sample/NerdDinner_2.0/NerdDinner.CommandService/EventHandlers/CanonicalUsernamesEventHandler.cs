using System;
using NerdDinner.CommandService.Events;
using SimpleCqrs.Eventing;

namespace NerdDinner.CommandService.EventHandlers
{
    public class CanonicalUsernamesEventHandler : IHandleDomainEvents<UserCreatedEvent>
    {
        public void Handle(UserCreatedEvent domainEvent)
        {
        }
    }
}