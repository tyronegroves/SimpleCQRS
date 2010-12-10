using System;
using NServiceBus;
using SimpleCqrs.Commanding;

namespace SimpleCqrs.NServiceBus.Commanding
{
    public static class ConfigureCommandBus
    {
        public static ConfigCommandBus CommandBus(this Configure configure)
        {
            var configCommandBus = new ConfigCommandBus();
            configCommandBus.Configure(configure);

            return configCommandBus;
        }

        public static ConfigCommandBus ExeucteTimeout(this ConfigCommandBus configure, int millisecondsTimeout)
        {
            configure.ExecuteCommandTimeout = TimeSpan.FromMilliseconds(millisecondsTimeout);
            return configure;
        }

        public static ConfigCommandBus ExeucteTimeout(this ConfigCommandBus configure, TimeSpan timeout)
        {
            configure.ExecuteCommandTimeout = timeout;
            return configure;
        }

        public static ICommandBus Create(this ConfigCommandBus configure)
        {
            var bus = configure
                .MsmqTransport()
                .UnicastBus()
                    .CreateBus();

            var commandBus = new NsbCommandBus((IBus)bus, configure.CommandTypeToDestinationLookup, configure.ExecuteCommandTimeout);
            configure.Configurer.RegisterSingleton<ICommandBus>(commandBus);

            bus.Start();

            return commandBus;
        }
    }
}