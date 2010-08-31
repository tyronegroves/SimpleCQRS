using System;
using SimpleCqrs.Commanding;

namespace Commands
{
    public class AddProductToCartCommand : Command
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}