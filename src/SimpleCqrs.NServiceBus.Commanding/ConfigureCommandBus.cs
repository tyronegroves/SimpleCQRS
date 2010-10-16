using NServiceBus;
using SimpleCqrs.Commanding;

namespace SimpleCqrs.NServiceBus.Commanding
{
    public static class ConfigureCommandBus
    {
        public static ICommandBus CommandBus(this Configure configure)
        {
            configure.UnicastBus();

            var configCommandBus = new ConfigCommandBus();
            configCommandBus.Configure(configure);
            var unicastBus = configure.CreateBus();
            unicastBus.Start();

            var commandBus = new CommandBus((IBus)unicastBus, configCommandBus.CommandTypeToDestinationLookup);
            configure.Configurer.RegisterSingleton<ICommandBus>(commandBus);
            return commandBus;
        }
    }
}