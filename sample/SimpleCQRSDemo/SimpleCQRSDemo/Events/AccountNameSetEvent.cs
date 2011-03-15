using SimpleCqrs.Eventing;

namespace SimpleCQRSDemo.Events
{
    public class AccountNameSetEvent : DomainEvent
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}