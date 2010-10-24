using NServiceBus;
using SimpleCqrs.Commanding;

namespace SimpleCqrs.NServiceBus.Commanding
{
    public static class ConfigureCommandBus
    {
        public static ICommandBus CommandBus(this Configure configure)
        {
            var configCommandBus = new ConfigCommandBus();
            configCommandBus.Configure(configure);

            var bus = configure
                .MsmqTransport()
                .UnicastBus()
                    .CreateBus();

            var commandBus = new NsbCommandBus((IBus)bus, configCommandBus.CommandTypeToDestinationLookup);
            configure.Configurer.RegisterSingleton<ICommandBus>(commandBus);

            bus.Start();

            return commandBus;
        }
    }
}