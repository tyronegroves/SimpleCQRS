using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SimpleCqrs.Commands
{
    internal class DirectCommandBus : ICommandBus
    {
        private readonly IServiceLocator serviceLocator;
        private IDictionary<Type, List<CommandHandlerInfo>> commandHandlerMap;

        public DirectCommandBus(ITypeCatalog typeCatalog, IServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
            BuildCommandHandlerMap(typeCatalog.GetGenericInterfaceImplementations(typeof(IHandleCommands<>)));
        }

        public void Execute(Command command)
        {
            var commandHandlerInfoList = commandHandlerMap[command.GetType()];
            foreach (var commandHandlerInfo in commandHandlerInfoList)
            {
                var commandHandler = serviceLocator.Resolve(commandHandlerInfo.CommandHandlerType);
                commandHandlerInfo.HandleMethod.Invoke(commandHandler, new object[] {command});
            }
        }

        private void BuildCommandHandlerMap(IEnumerable<Type> commandHandlerTypes)
        {
            commandHandlerMap = new Dictionary<Type, List<CommandHandlerInfo>>();
            foreach (var commandHandlerType in commandHandlerTypes)
            {
                foreach (var commandType in GetCommadTypes(commandHandlerType))
                {
                    if (!commandHandlerMap.ContainsKey(commandType))
                        commandHandlerMap.Add(commandType, new List<CommandHandlerInfo>());

                    var commandHandlerInfoList = commandHandlerMap[commandType];

                    var handleMethod = typeof(IHandleCommands<>).MakeGenericType(commandType).GetMethod("Handle");
                    commandHandlerInfoList.Add(new CommandHandlerInfo {CommandHandlerType = commandHandlerType, HandleMethod = handleMethod});
                }
            }
        }

        private class CommandHandlerInfo
        {
            public Type CommandHandlerType { get; set; }
            public MethodInfo HandleMethod { get; set; }
        }

        private static IEnumerable<Type> GetCommadTypes(Type commandHandlerType)
        {
            return from interfaceType in commandHandlerType.GetInterfaces()
                   where interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IHandleCommands<>)
                   select interfaceType.GetGenericArguments()[0];
        }
    }
}