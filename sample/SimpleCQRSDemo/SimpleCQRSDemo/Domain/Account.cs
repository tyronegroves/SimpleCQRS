using System;
using SimpleCqrs.Domain;
using SimpleCQRSDemo.Events;

namespace SimpleCQRSDemo.Domain
{
    public class Account : AggregateRoot
    {
        public Account(Guid id)
        {
            Apply(new AccountCreatedEvent { AggregateRootId = id });
        }

        public void SetName(string firstName, string lastName)
        {
            Apply(new AccountNameSetEvent{ FirstName = firstName, LastName = lastName});
        }

        public void OnAccountCreated(AccountCreatedEvent evt)
        {
            Id = evt.AggregateRootId;
        }
    }
}