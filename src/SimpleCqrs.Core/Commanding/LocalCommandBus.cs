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
            BuildCommandInvokers(typeCatalog.GetGenericInterfaceImplementations(typeof(IHandleCommands<>)));
        }

        public int Execute(ICommand command)
        {
            CommandHandlerInvoker commandInvoker;
            if(!commandInvokers.TryGetValue(command.GetType(), out commandInvoker))
                throw new CommandHandlerNotFoundException(command.GetType());

            return commandInvoker.Execute(command);
        }

        private void BuildCommandInvokers(IEnumerable<Type> commandHandlerTypes)
        {
            commandInvokers = new Dictionary<Type, CommandHandlerInvoker>();
            foreach(var commandHandlerType in commandHandlerTypes)
            {
                var commandTypes = GetCommadTypesForCommandHandler(commandHandlerType);
                foreach(var commandType in commandTypes)
                {
                    if(commandInvokers.ContainsKey(commandType))
                        throw new DuplicateCommandHandlersException(commandType);

                    commandInvokers.Add(commandType, new CommandHandlerInvoker(serviceLocator, commandType, commandHandlerType));
                }
            }
        }

        private static IEnumerable<Type> GetCommadTypesForCommandHandler(Type commandHandlerType)
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
            private readonly IEnumerable<ICommandErrorHandler<ICommand>> errorHandlers;

            public CommandHandlerInvoker(IServiceLocator serviceLocator, Type commandType, Type commandHandlerType)
            {
                this.serviceLocator = serviceLocator;
                this.commandType = commandType;
                this.commandHandlerType = commandHandlerType;
                errorHandlers = GetCommandErrorHandlersForCommandType(serviceLocator, commandType);
            }

            private static IEnumerable<ICommandErrorHandler<ICommand>> GetCommandErrorHandlersForCommandType(IServiceLocator serviceLocator, Type commandType)
            {
                var typeCatalog = serviceLocator.Resolve<ITypeCatalog>();
                var errorHandlerTypes = typeCatalog.GetGenericInterfaceImplementations(typeof(ICommandErrorHandler<>));
                var commandErrorHandlers = new List<ICommandErrorHandler<ICommand>>();

                foreach(var errorHandlerType in errorHandlerTypes)
                {
                    var errorHandlerCommandTypes = GetCommadTypesForCommandErrorHandler(errorHandlerType);
                    foreach(var errorHandlerCommandType in errorHandlerCommandTypes)
                    {
                        if(commandType.IsAssignableFrom(errorHandlerCommandType))
                        {
                            commandErrorHandlers.Add((ICommandErrorHandler<ICommand>)serviceLocator.Resolve(errorHandlerType));
                        }
                    }
                }
                return commandErrorHandlers;
            }

            private static IEnumerable<Type> GetCommadTypesForCommandErrorHandler(Type commandErrorHandlerType)
            {
                return (from interfaceType in commandErrorHandlerType.GetInterfaces()
                        where interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(ICommandErrorHandler<>)
                        select interfaceType.GetGenericArguments()[0]).ToArray();
            }

            public int Execute(ICommand command)
            {
                var handleMethod = typeof(IHandleCommands<>).MakeGenericType(commandType).GetMethod("Handle");
                var commandHandler = serviceLocator.Resolve(commandHandlerType);

                var handlingContextType = typeof(CommandHandlingContext<>).MakeGenericType(commandType);
                var handlingContext = (ICommandHandlingContext<ICommand>)Activator.CreateInstance(handlingContextType, command);

                ThreadPool.QueueUserWorkItem(delegate
                                                 {
                                                     try
                                                     {
                                                         handleMethod.Invoke(commandHandler, new object[] { handlingContext });
                                                     }
                                                     catch (Exception exception)
                                                     {
                                                         errorHandlers.ForEach(handler => handler.Handle(handlingContext, exception));
                                                     }
                                                     finally
                                                     {
                                                         ((ICommandHandlingContext)handlingContext).WaitHandle.Set();
                                                     }
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