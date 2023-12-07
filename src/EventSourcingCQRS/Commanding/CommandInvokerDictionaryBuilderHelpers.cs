namespace EventSourcingCQRS.Commanding
{
    internal static class CommandInvokerDictionaryBuilderHelpers
    {
        public static IDictionary<Type, LocalCommandBus.CommandHandlerInvoker> CreateADictionaryOfCommandInvokers(
            ITypeCatalog typeCatalog, IServiceLocator serviceLocator)
        {
            var types = GetAllCommandHandlerTypes(typeCatalog);
            return CreateCommandInvokersForTheseTypes(types, serviceLocator);
        }

        private static IEnumerable<Type> GetAllCommandHandlerTypes(ITypeCatalog typeCatalog)
        {
            return typeCatalog.GetGenericInterfaceImplementations(typeof (IHandleCommands<>));
        }

        private static IDictionary<Type, LocalCommandBus.CommandHandlerInvoker> CreateCommandInvokersForTheseTypes(
            IEnumerable<Type> commandHandlerTypes, IServiceLocator serviceLocator)
        {
            var commandInvokerDictionary = new Dictionary<Type, LocalCommandBus.CommandHandlerInvoker>();
            foreach (var commandHandlerType in commandHandlerTypes)
            {
                var commandTypes = GetCommandTypesForCommandHandler(commandHandlerType);
                foreach (var commandType in commandTypes)
                {
                    if (commandInvokerDictionary.ContainsKey(commandType))
                        throw new DuplicateCommandHandlersException(commandType);

                    commandInvokerDictionary.Add(commandType,
                                                 new LocalCommandBus.CommandHandlerInvoker(serviceLocator, commandType,
                                                                                           commandHandlerType));
                }
            }
            return commandInvokerDictionary;
        }

        private static IEnumerable<Type> GetCommandTypesForCommandHandler(Type commandHandlerType)
        {
            return (from interfaceType in commandHandlerType.GetInterfaces()
                    where
                        interfaceType.IsGenericType &&
                        interfaceType.GetGenericTypeDefinition() == typeof (IHandleCommands<>)
                    select interfaceType.GetGenericArguments()[0]).ToArray();
        }
    }
}