using System;
using SimpleCqrs.Eventing;

namespace Events
{
    public class ProductAddedToCartEvent : DomainEvent
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}