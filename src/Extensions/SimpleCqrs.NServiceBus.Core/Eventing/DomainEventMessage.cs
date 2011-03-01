using System;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.NServiceBus.Eventing
{
    [Serializable]
    public class DomainEventMessage<TDomainEvent> : IDomainEventMessage
        where TDomainEvent : DomainEvent
    {
        public TDomainEvent DomainEvent { get; set; }

        public string Header { get; set; }

        DomainEvent IDomainEventMessage.DomainEvent
        {
            get { return DomainEvent; }
            set { DomainEvent = (TDomainEvent)value; }
        }
    }
}