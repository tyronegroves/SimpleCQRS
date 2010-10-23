using NServiceBus;
using SimpleCqrs.NServiceBus.Eventing;

namespace Server
{
    public class MyTestEventMessageHandler : IHandleMessages<IDomainEventMessage>
    {
        public void Handle(IDomainEventMessage message)
        {
        }
    }
}