using System;
using System.Collections.Generic;
using System.Linq;
using NServiceBus;
using SimpleCqrs.Commanding;
using SimpleCqrs.NServiceBus.Commanding.Config;

namespace SimpleCqrs.NServiceBus.Commanding
{
    public class ConfigCommandBus : Configure
    {
        private readonly IDictionary<Type, string> commandTypeToDestinationLookup = new Dictionary<Type, string>();

        public IDictionary<Type, string> CommandTypeToDestinationLookup
        {
            get { return commandTypeToDestinationLookup; }
        }

        public void Configure(Configure config)
        {
            Configurer = config.Configurer;
            Builder = config.Builder;

            var commandBusConfig = GetConfigSection<CommandBusConfig>();
            var commandTypes = TypesToScan
                .Where(type => typeof(ICommand).IsAssignableFrom(type))
                .ToList();

            foreach(CommandEndpointMapping mapping in commandBusConfig.CommandEndpointMappings)
            {
                foreach(var commandType in commandTypes)
                {
                    if(CommandsIsTypeNameOrAssemblieNameForCommandType(commandType, mapping.Commands))
                    {
                        commandTypeToDestinationLookup.Add(commandType, mapping.Endpoint);
                    }
                }
            }
        }

        private static bool CommandsIsTypeNameOrAssemblieNameForCommandType(Type commandType, string commands)
        {
            commands = commands.ToLower();
            return commandType.AssemblyQualifiedName.ToLower() == commands
                || commandType.Assembly.GetName().Name.ToLower() == commands
                || commandType.FullName == commands;
        }
    }
}