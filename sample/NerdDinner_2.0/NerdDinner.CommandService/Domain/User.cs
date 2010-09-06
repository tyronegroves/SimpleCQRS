using System;
using NerdDinner.CommandService.Events;
using SimpleCqrs.Domain;

namespace NerdDinner.CommandService.Domain
{
    public class User : AggregateRoot
    {
        public User()
        {   
        }

        public User(Guid aggregateRootId)
        {
            Apply(new UserCreatedEvent {AggregateRootId = aggregateRootId});
        }

        private void OnUserCreated(UserCreatedEvent domainEvent)
        {
            Id = domainEvent.AggregateRootId;
        }
    }
}