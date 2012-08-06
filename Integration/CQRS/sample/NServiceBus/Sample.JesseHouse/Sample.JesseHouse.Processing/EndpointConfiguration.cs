using NServiceBus;
using SimpleCqrs;
using SimpleCqrs.NServiceBus;
using SimpleCqrs.Unity;

namespace Sample.JesseHouse.Processing
{
    public class EndpointConfiguration : IConfigureThisEndpoint, AsA_Publisher, IWantCustomInitialization
    {
        public void Init()
        {
            Configure.With()
                .DefaultBuilder()
                .BinarySerializer()
                .SimpleCqrs(new SimpleCqrsRuntime<UnityServiceLocator>())
                    .UseLocalCommandBus()       // Tell SimpleCqrs to dispatch command locally (in this application)
                    .UseNsbEventBus();          // Tell SimpleCqrs to use NServiceBus to publish domain events

            // TODO: Go to Sample.JesseHouse.ViewModel project's EndpointConfiguration class
        }
    }
}