using System;
using SimpleCqrs.Eventing;

namespace Sample.JesseHouse.Events
{
    [Serializable]
    public class CustomerNameHasChangedEvent : DomainEvent
    {
        public Guid CustomerId
        {
            get { return AggregateRootId; }
            set { AggregateRootId = value; }
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}