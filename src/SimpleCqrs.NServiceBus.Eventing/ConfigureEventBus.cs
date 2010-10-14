using NServiceBus;

namespace SimpleCqrs.NServiceBus.Eventing
{
    public static class ConfigureEventBus
    {
        public static ConfigEventBus EventBus(this Configure configure)
        {
            var eventBus = new ConfigEventBus();
            eventBus.Configure(configure);
            return eventBus;
        }
    }
}