using System;
using SimpleCqrs.Eventing;

namespace NerdDinner.CommandService.Events
{
    public class DinnerEventDateChangedEvent : DomainEvent
    {
        public Guid DinnerId
        {
            get { return AggregateRootId; }
            set { AggregateRootId = value; }
        }

        public DateTime NewEventDate { get; set; }
        public DateTime PreviousEventDate { get; set; }
    }
}