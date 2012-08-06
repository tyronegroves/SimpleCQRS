using System;
using SimpleCqrs.Eventing;

namespace NerdDinner.CommandService.Events
{
    public class UserMemberStatusChangedEvent : DomainEvent
    {
        public Guid UserId
        {
            get { return AggregateRootId; }
            set { AggregateRootId = value; }
        }

        public string Status { get; set; }
    }
}