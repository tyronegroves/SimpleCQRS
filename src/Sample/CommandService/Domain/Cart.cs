using System;
using System.Collections.Generic;
using Events;
using SimpleCqrs.Domain;

namespace Commands.Domain
{
    public class Cart : AggregateRoot
    {
        private List<CartProduct> products = new List<CartProduct>();

        public static Cart Create(Guid cartId)
        {
            var cart = new Cart();
            cart.Apply(new CartCreatedEvent {Id = cartId});
            return cart;
        }

        public void AddProduct(Guid productId, int quantity)
        {
            Apply(new ProductAddedToCartEvent{ProductId = productId, Quantity = quantity});
        }

        private void OnCartCreated(CartCreatedEvent domainEvent)
        {
            Id = domainEvent.Id;
        }

        private void OnProductAddedToCart(ProductAddedToCartEvent domainEvent)
        {
            products.Add(new CartProduct{ProductId = domainEvent.ProductId, Quantity = domainEvent.Quantity});
        }

        public class CartProduct
        {
            public Guid ProductId { get; set; }
            public int Quantity { get; set; }
        }
    }
}