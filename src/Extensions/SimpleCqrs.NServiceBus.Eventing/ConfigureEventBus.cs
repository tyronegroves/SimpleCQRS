using System;
using NServiceBus.Unicast.Config;

namespace SimpleCqrs.NServiceBus.Eventing
{
    public static class ConfigureEventBus
    {
        public static ConfigUnicastBus SubscribeForDomainEvents(this ConfigUnicastBus configure)
        {
            return SubscribeForDomainEvents(configure, new NsbServiceLocator(configure.Configurer, configure.Builder));
        }

        public static ConfigUnicastBus SubscribeForDomainEvents<TServiceLocator>(this ConfigUnicastBus configure) where TServiceLocator : IServiceLocator
        {
            return SubscribeForDomainEvents(configure, Activator.CreateInstance<TServiceLocator>());
        }

        public static ConfigUnicastBus SubscribeForDomainEvents(this ConfigUnicastBus configure, IServiceLocator serviceLocator)
        {
            var eventBus = new ConfigEventBus();
            eventBus.Configure(configure, serviceLocator);

            return configure;
        }
    }
}