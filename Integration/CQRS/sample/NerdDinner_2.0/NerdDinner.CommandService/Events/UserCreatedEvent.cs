using System;
using SimpleCqrs.Eventing;

namespace NerdDinner.CommandService.Events
{
    public class UserCreatedEvent : DomainEvent
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        public Guid UserId
        {
            get { return AggregateRootId; }
            set { AggregateRootId = value; }
        }
    }
}