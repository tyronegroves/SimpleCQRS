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

        public TimeSpan ExecuteCommandTimeout { get; set; }

        public void Configure(Configure config)
        {
            Configurer = config.Configurer;
            Builder = config.Builder;

            ExecuteCommandTimeout = TimeSpan.FromSeconds(10);
            var commandBusConfig = GetConfigSection<CommandBusConfig>();
            var commandTypes = TypesToScan
                .Where(type => typeof(ICommand).IsAssignableFrom(type))
                .ToList();

            RegisterAssemblyCommandDestinationMappings(commandBusConfig, commandTypes);
            RegisterCommandDestinationMappings(commandBusConfig, commandTypes);
        }

        private void RegisterCommandDestinationMappings(CommandBusConfig commandBusConfig, IEnumerable<Type> commandTypes)
        {
            foreach (var mapping in GetCommandEndpointMappingsForCommand(commandBusConfig.CommandEndpointMappings, commandTypes))
            {
                foreach (var commandType in commandTypes)
                {
                    if (CommandTypeIsCommand(commandType, mapping.Commands))
                    {
                        if (commandTypeToDestinationLookup.ContainsKey(commandType))
                            commandTypeToDestinationLookup[commandType] = mapping.Endpoint;
                        else
                            commandTypeToDestinationLookup.Add(commandType, mapping.Endpoint);
                    }
                }
            }
        }

        private void RegisterAssemblyCommandDestinationMappings(CommandBusConfig commandBusConfig, IEnumerable<Type> commandTypes)
        {
            foreach(var mapping in GetCommandEndpointMappingsForAssembly(commandBusConfig.CommandEndpointMappings, commandTypes))
            {
                foreach(var commandType in commandTypes)
                {
                    if(CommandTypeIsInCommandAssembly(commandType, mapping.Commands))
                    {
                        commandTypeToDestinationLookup.Add(commandType, mapping.Endpoint);
                    }
                }
            }
        }

        private static IEnumerable<CommandEndpointMapping> GetCommandEndpointMappingsForAssembly(CommandEndpointMappingCollection commandEndpointMappings, IEnumerable<Type> commandTypes)
        {
            return commandEndpointMappings
                .Cast<CommandEndpointMapping>()
                .Where(mapping => commandTypes.Any(t => t.Assembly.GetName().Name.Equals(mapping.Commands,StringComparison.InvariantCultureIgnoreCase)));
        }

        private static IEnumerable<CommandEndpointMapping> GetCommandEndpointMappingsForCommand(CommandEndpointMappingCollection commandEndpointMappings, IEnumerable<Type> commandTypes)
        {
            return commandEndpointMappings
                .Cast<CommandEndpointMapping>()
                .Where(mapping => commandTypes.Any(t => t.FullName.Equals(mapping.Commands, StringComparison.InvariantCultureIgnoreCase) || t.Name.Equals(mapping.Commands, StringComparison.InvariantCultureIgnoreCase)));
        }

        private static bool CommandTypeIsInCommandAssembly(Type commandType, string commandAssembly)
        {
            return commandType.Assembly.GetName().Name.ToLower() == commandAssembly.ToLower();
        }

        private static bool CommandTypeIsCommand(Type commandType, string command)
        {
            return commandType.FullName.ToLower() == command.ToLower()
                   || commandType.AssemblyQualifiedName.ToLower() == command.ToLower();
        }
    }
}