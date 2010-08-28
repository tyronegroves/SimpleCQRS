using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCqrs.Commands
{
    internal class DirectCommandBus : ICommandBus
    {
        private readonly IServiceLocator serviceLocator;
        private IDictionary<Type, CommandInvoker> commandInvokers;

        public DirectCommandBus(ITypeCatalog typeCatalog, IServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
            BuildCommandInvokers(typeCatalog.GetGenericInterfaceImplementations(typeof(IHandleCommands<>)));
        }

        public void Execute(Command command)
        {
            var commandInvoker = commandInvokers[command.GetType()];
            commandInvoker.Execute(command);
        }

        private void BuildCommandInvokers(IEnumerable<Type> commandHandlerTypes)
        {
            commandInvokers = new Dictionary<Type, CommandInvoker>();
            foreach (var commandHandlerType in commandHandlerTypes)
            {
                foreach (var commandType in GetCommadTypes(commandHandlerType))
                {
                    CommandInvoker commandInvoker;
                    if (!commandInvokers.TryGetValue(commandType, out commandInvoker))
                        commandInvoker = new CommandInvoker(serviceLocator, commandType);

                    commandInvoker.AddCommandHandlerType(commandHandlerType);
                    commandInvokers[commandType] = commandInvoker;
                }
            }
        }

        private static IEnumerable<Type> GetCommadTypes(Type commandHandlerType)
        {
            return from interfaceType in commandHandlerType.GetInterfaces()
                   where interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IHandleCommands<>)
                   select interfaceType.GetGenericArguments()[0];
        }

        private class CommandInvoker
        {
            private readonly IServiceLocator serviceLocator;
            private readonly Type commandType;
            private readonly List<Type> commandHandlerTypes;

            public CommandInvoker(IServiceLocator serviceLocator, Type commandType)
            {
                this.serviceLocator = serviceLocator;
                this.commandType = commandType;
                commandHandlerTypes = new List<Type>();
            }

            public void AddCommandHandlerType(Type commandHandlerType)
            {
                commandHandlerTypes.Add(commandHandlerType);
            }

            public void Execute(Command command)
            {
                var handleMethod = typeof(IHandleCommands<>).MakeGenericType(commandType).GetMethod("Handle");
                foreach (var commandHandlerType in commandHandlerTypes)
                {
                    var commandHandler = serviceLocator.Resolve(commandHandlerType);
                    handleMethod.Invoke(commandHandler, new object[] {command});
                }
            }
        }
    }
}