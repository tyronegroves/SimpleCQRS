using NServiceBus;

namespace SimpleCqrs.NServiceBus
{
    public static class ConfigureSimpleCqrs
    {
        public static ConfigSimpleCqrs SimpleCqrs(this Configure configure, ISimpleCqrsRuntime runtime)
        {
            var configSimpleCqrs = new ConfigSimpleCqrs(runtime);
            configSimpleCqrs.Configure(configure);
            return configSimpleCqrs;
        }
    }
}