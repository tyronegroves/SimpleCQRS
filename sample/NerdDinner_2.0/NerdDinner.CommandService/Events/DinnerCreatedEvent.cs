using System;
using SimpleCqrs.Eventing;

namespace NerdDinner.CommandService.Events
{
    public class DinnerCreatedEvent : DomainEvent
    {
        public Guid DinnerId
        {
            get { return AggregateRootId; }
            set { AggregateRootId = value; }
        }

        public DateTime DinnerDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ContactPhone { get; set; }
    }
}