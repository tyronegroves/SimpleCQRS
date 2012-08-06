using NServiceBus;
using SimpleCqrs;
using SimpleCqrs.NServiceBus;
using SimpleCqrs.Unity;

namespace Sample.JesseHouse.ViewModel
{
    public class EndpointConfiguration : IConfigureThisEndpoint, AsA_Server, IWantCustomInitialization
    {
        public void Init()
        {
            Configure.With()
                .DefaultBuilder()
                .BinarySerializer()
                .SimpleCqrs(new SimpleCqrsRuntime<UnityServiceLocator>())
                    .SubscribeForDomainEvents();    // Tells SimpleCqrs to subscribe for domain events

            // The DomainEventBusConfig element in the Web.config tell SimpleCqrs which domain events to listen for and the queue 
            // to listen for them from.
        }
    }
}