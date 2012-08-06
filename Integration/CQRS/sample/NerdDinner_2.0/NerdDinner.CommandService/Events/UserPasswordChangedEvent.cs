using System;
using SimpleCqrs.Eventing;

namespace NerdDinner.CommandService.Events
{
    public class UserPasswordChangedEvent : DomainEvent
    {
        public string NewPassword { get; set; }
        
        public Guid UserId
        {
            get { return AggregateRootId; }
            set { AggregateRootId = value; }
        }
    }
}