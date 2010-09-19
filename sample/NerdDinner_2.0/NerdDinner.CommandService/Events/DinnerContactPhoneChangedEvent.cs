using System;
using SimpleCqrs.Eventing;

namespace NerdDinner.CommandService.Events
{
    public class DinnerContactPhoneChangedEvent : DomainEvent
    {
        public Guid DinnerId
        {
            get { return AggregateRootId; }
            set { AggregateRootId = value; }
        }

        public string NewContactPhone { get; set; }
        public string PreviousContactPhone { get; set; }
    }
}