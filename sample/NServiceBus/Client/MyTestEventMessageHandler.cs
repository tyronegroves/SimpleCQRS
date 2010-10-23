using System;
using Events;
using SimpleCqrs.Eventing;

namespace Client
{
    public class MyTestEventMessageHandler : IHandleDomainEvents<MyTestEvent>, IHandleDomainEvents<MyTestEvent2>
    {
        public void Handle(MyTestEvent domainEvent)
        {
        }

        public void Handle(MyTestEvent2 domainEvent)
        {
        }
    }
}