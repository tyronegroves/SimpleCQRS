using System;
using SimpleCqrs.Eventing;

namespace NerdDinner.CommandService.Events
{
    public class DinnerChangedEvent : DomainEvent
    {
        public Guid DinnerId
        {
            get { return AggregateRootId; }
            set { AggregateRootId = value; }
        }

        public string NewTitle { get; set; }
        public string NewDescription { get; set; }
        public string PreviousTitle { get; set; }
        public string PreviousDescription { get; set; }
    }
}