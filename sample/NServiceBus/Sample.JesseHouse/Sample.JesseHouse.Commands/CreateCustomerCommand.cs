using System;
using SimpleCqrs.Commanding;

namespace Sample.JesseHouse.Commands
{
    [Serializable]
    public class CreateCustomerCommand : ICommand
    {
        public Guid CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}