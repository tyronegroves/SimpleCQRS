using System;
using SimpleCqrs.Eventing;

namespace NerdDinner.CommandService.Events
{
    public class DinnerLocationChangedEvent : DomainEvent
    {
        public Guid DinnerId
        {
            get { return AggregateRootId; }
            set { AggregateRootId = value; }
        }

        public string Address { get; set; }
        public string Country { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}