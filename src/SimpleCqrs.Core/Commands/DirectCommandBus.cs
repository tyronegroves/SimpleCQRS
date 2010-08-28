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

        public int Execute(Command command)
        {
            var commandInvoker = commandInvokers[command.GetType()];
            return commandInvoker.Execute(command);
        }

        private void BuildCommandInvokers(IEnumerable<Type> commandHandlerTypes)
        {
            commandInvokers = new Dictionary<Type, CommandInvoker>();
            foreach(var commandHandlerType in commandHandlerTypes)
            {
                var commandTypes = GetCommadTypesForCommandHandler(commandHandlerType);
                foreach(var commandType in commandTypes)
                {
                    if(commandInvokers.ContainsKey(commandType))
                        throw new DuplicateCommandHandlersException(commandType);

                    commandInvokers.Add(commandType, new CommandInvoker(serviceLocator, commandType, commandHandlerType));
                }
            }
        }

        private static IEnumerable<Type> GetCommadTypesForCommandHandler(Type commandHandlerType)
        {
            return (from interfaceType in commandHandlerType.GetInterfaces()
                    where interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IHandleCommands<>)
                    select interfaceType.GetGenericArguments()[0]).ToArray();
        }

        private class CommandInvoker
        {
            private readonly Type commandHandlerType;
            private readonly Type commandType;
            private readonly IServiceLocator serviceLocator;

            public CommandInvoker(IServiceLocator serviceLocator, Type commandType, Type commandHandlerType)
            {
                this.serviceLocator = serviceLocator;
                this.commandType = commandType;
                this.commandHandlerType = commandHandlerType;
            }

            public int Execute(Command command)
            {
                var handleMethod = typeof(IHandleCommands<>).MakeGenericType(commandType).GetMethod("Handle");
                var commandHandler = serviceLocator.Resolve(commandHandlerType);
                return (int)handleMethod.Invoke(commandHandler, new object[] {command});
            }
        }
    }
}