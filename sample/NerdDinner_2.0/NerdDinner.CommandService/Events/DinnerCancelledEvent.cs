using System;
using SimpleCqrs.Eventing;

namespace NerdDinner.CommandService.Events
{
    public class DinnerCancelledEvent : DomainEvent
    {
        public Guid DinnerId
        {
            get { return AggregateRootId; }
            set { AggregateRootId = value; }
        }
    }
}