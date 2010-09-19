using System;
using SimpleCqrs.Eventing;

namespace NerdDinner.CommandService.Domain
{
    public class DinnerHostChangedEvent : DomainEvent
    {
        public Guid DinnerId
        {
            get { return AggregateRootId; }
            set { AggregateRootId = value; }
        }

        public Guid HostedById { get; set; }
        public string HostedBy { get; set; }
    }
}