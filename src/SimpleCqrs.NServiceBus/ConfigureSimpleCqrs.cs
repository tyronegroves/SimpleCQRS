using NServiceBus;

namespace SimpleCqrs.NServiceBus
{
    public static class ConfigureSimpleCqrs
    {
        public static ConfigSimpleCqrs<TServiceLocator> SimpleCqrs<TServiceLocator>(this Configure configure, TServiceLocator serviceLocator) where TServiceLocator : class, IServiceLocator
        {
            var runtime = new NServiceBusSimpleCqrsRuntime<TServiceLocator> {ServiceLocator = serviceLocator};
            var simpleCqrs = new ConfigSimpleCqrs<TServiceLocator>(runtime);
            simpleCqrs.Configure(configure);
            return simpleCqrs;
        }

        public static ConfigSimpleCqrs<TServiceLocator> SimpleCqrs<TServiceLocator>(this Configure configure) where TServiceLocator : class, IServiceLocator
        {
            return SimpleCqrs<TServiceLocator>(configure, null);
        }
    }
}