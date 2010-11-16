using System;
using System.Collections.Generic;
using System.Reflection;

namespace SimpleCqrs.Commanding
{
    internal class LocalCommandBus : ICommandBus
    {
        private readonly IDictionary<Type, CommandHandlerInvoker> commandInvokers;

        public LocalCommandBus(ITypeCatalog typeCatalog, IServiceLocator serviceLocator)
        {
            commandInvokers =
                CommandInvokerDictionaryBuilderHelpers.CreateADictionaryOfCommandInvokers(typeCatalog, serviceLocator);
        }

        public int Execute<TCommand>(TCommand command) where TCommand : ICommand
        {
            var commandHandler = GetTheCommandHandler(command);
            return commandHandler.Execute(command);
        }

        public void Send<TCommand>(TCommand command) where TCommand : ICommand
        {
            var commandHandler = GetTheCommandHandler(command);
            commandHandler.Send(command);
        }

        private CommandHandlerInvoker GetTheCommandHandler(ICommand command)
        {
            CommandHandlerInvoker commandInvoker;
            if(!commandInvokers.TryGetValue(command.GetType(), out commandInvoker))
                throw new CommandHandlerNotFoundException(command.GetType());
            return commandInvoker;
        }

        public class CommandHandlerInvoker
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
                var handlingContext = CreateTheCommandHandlingContext(command);
                ExecuteTheCommandHandler(handlingContext);
                return ((ICommandHandlingContext)handlingContext).ReturnValue;
            }

            public void Send(ICommand command)
            {
                var handlingContext = CreateTheCommandHandlingContext(command);
                ExecuteTheCommandHandler(handlingContext);
            }

            private void ExecuteTheCommandHandler(ICommandHandlingContext<ICommand> handlingContext)
            {
                var handleMethod = GetTheHandleMethod();
                var commandHandler = CreateTheCommandHandler();
                handleMethod.Invoke(commandHandler, new object[] {handlingContext});
            }

            private ICommandHandlingContext<ICommand> CreateTheCommandHandlingContext(ICommand command)
            {
                var handlingContextType = typeof(CommandHandlingContext<>).MakeGenericType(commandType);
                return (ICommandHandlingContext<ICommand>)Activator.CreateInstance(handlingContextType, command);
            }

            private object CreateTheCommandHandler()
            {
                return serviceLocator.Resolve(commandHandlerType);
            }

            private MethodInfo GetTheHandleMethod()
            {
                return typeof(IHandleCommands<>).MakeGenericType(commandType).GetMethod("Handle");
            }
        }

        private interface ICommandHandlingContext
        {
            ICommand Command { get; }
            int ReturnValue { get; }
        }

        private class CommandHandlingContext<TCommand> : ICommandHandlingContext, ICommandHandlingContext<TCommand>
            where TCommand : ICommand
        {
            public CommandHandlingContext(TCommand command)
            {
                Command = command;
            }

            public TCommand Command { get; private set; }

            ICommand ICommandHandlingContext.Command
            {
                get { return Command; }
            }

            public int ReturnValue { get; private set; }

            public void Return(int value)
            {
                ReturnValue = value;
            }
        }
    }
}