using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SimpleCqrs.Commanding
{
    internal class LocalCommandBus : ICommandBus
    {
        private readonly IServiceLocator serviceLocator;
        private IDictionary<Type, CommandHandlerInvoker> commandInvokers;

        public LocalCommandBus(ITypeCatalog typeCatalog, IServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;

            var types = GetAllCommandHandlerTypes(typeCatalog);
            commandInvokers = CreateCommandInvokersForTheseTypes(types);
        }

        private static IEnumerable<Type> GetAllCommandHandlerTypes(ITypeCatalog typeCatalog)
        {
            return typeCatalog.GetGenericInterfaceImplementations(typeof(IHandleCommands<>));
        }

        public int Execute(ICommand command)
        {
            var commandHandler = GetTheCommandHandler(command);

            return commandHandler.Execute(command);
        }

        private CommandHandlerInvoker GetTheCommandHandler(ICommand command)
        {
            CommandHandlerInvoker commandInvoker;
            if(!commandInvokers.TryGetValue(command.GetType(), out commandInvoker))
                throw new CommandHandlerNotFoundException(command.GetType());
            return commandInvoker;
        }

        private IDictionary<Type, CommandHandlerInvoker> CreateCommandInvokersForTheseTypes(IEnumerable<Type> commandHandlerTypes)
        {
            var commandInvokerDictionary = new Dictionary<Type, CommandHandlerInvoker>();
            foreach(var commandHandlerType in commandHandlerTypes)
            {
                var commandTypes = GetCommandTypesForCommandHandler(commandHandlerType);
                foreach(var commandType in commandTypes)
                {
                    if(commandInvokerDictionary.ContainsKey(commandType))
                        throw new DuplicateCommandHandlersException(commandType);

                    commandInvokerDictionary.Add(commandType, new CommandHandlerInvoker(serviceLocator, commandType, commandHandlerType));
                }
            }
            return commandInvokerDictionary;
        }

        private static IEnumerable<Type> GetCommandTypesForCommandHandler(Type commandHandlerType)
        {
            return (from interfaceType in commandHandlerType.GetInterfaces()
                    where interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IHandleCommands<>)
                    select interfaceType.GetGenericArguments()[0]).ToArray();
        }

        private class CommandHandlerInvoker
        {
            private readonly Type commandHandlerType;
            private readonly Type commandType;
            private readonly IServiceLocator serviceLocator;

            public CommandHandlerInvoker(IServiceLocator serviceLocator, Type commandType, Type commandHandlerType)
            {
                this.serviceLocator = serviceLocator;
                this.commandType = commandType;
                this.commandHandlerType = commandHandlerType;
            }

            public int Execute(ICommand command)
            {
                var handleMethod = typeof(IHandleCommands<>).MakeGenericType(commandType).GetMethod("Handle");
                var commandHandler = serviceLocator.Resolve(commandHandlerType);

                var handlingContextType = typeof(CommandHandlingContext<>).MakeGenericType(commandType);
                var handlingContext = (ICommandHandlingContext<ICommand>)Activator.CreateInstance(handlingContextType, command);

                ThreadPool.QueueUserWorkItem(delegate
                                                 {
                                                     handleMethod.Invoke(commandHandler, new object[] { handlingContext });
                                                     ((ICommandHandlingContext)handlingContext).WaitHandle.Set();
                                                 });
                ((ICommandHandlingContext)handlingContext).WaitHandle.WaitOne();

                return ((ICommandHandlingContext)handlingContext).ReturnValue;
            }
        }

        private interface ICommandHandlingContext
        {
            ICommand Command { get; }
            int ReturnValue { get; }
            ManualResetEvent WaitHandle { get; }
        }

        private class CommandHandlingContext<TCommand> : ICommandHandlingContext, ICommandHandlingContext<TCommand> where TCommand : ICommand
        {
            private readonly ManualResetEvent waitHandle;

            public CommandHandlingContext(TCommand command)
            {
                waitHandle = new ManualResetEvent(false);
                Command = command;
            }

            public TCommand Command { get; private set; }

            ICommand ICommandHandlingContext.Command
            {
                get { return Command; }
            }

            ManualResetEvent ICommandHandlingContext.WaitHandle
            {
                get { return waitHandle; }
            }

            public int ReturnValue { get; private set; }

            public void Return(int value)
            {
                ReturnValue = value;
                waitHandle.Set();
            }
        }
    }
}