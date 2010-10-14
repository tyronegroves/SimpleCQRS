using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace SimpleCqrs.Commanding
{
    internal class LocalCommandBus : ICommandBus
    {
        private readonly IDictionary<Type, CommandHandlerInvoker> commandInvokers;

        public LocalCommandBus(ITypeCatalog typeCatalog, IServiceLocator serviceLocator)
        {
            commandInvokers =
                CommandInvokerDictionaryBuilder.CreateADictionaryOfCommandInvokers(typeCatalog, serviceLocator);
        }

        public int Execute(ICommand command)
        {
            var commandHandler = GetTheCommandHandler(command);
            return commandHandler.Execute(command);
        }

        private CommandHandlerInvoker GetTheCommandHandler(ICommand command)
        {
            CommandHandlerInvoker commandInvoker;
            if (!commandInvokers.TryGetValue(command.GetType(), out commandInvoker))
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

                ThreadPool.QueueUserWorkItem(delegate
                                                 {
                                                     ExecuteTheCommandHandler(handlingContext);
                                                     SignalThatTheTreadIsComplete(handlingContext);
                                                 });
                WaitForTheThreadToComplete(handlingContext);

                return ((ICommandHandlingContext) handlingContext).ReturnValue;
            }

            private void ExecuteTheCommandHandler(ICommandHandlingContext<ICommand> handlingContext)
            {
                var handleMethod = GetTheHandleMethod();
                var commandHandler = CreateTheCommandHandler();
                handleMethod.Invoke(commandHandler, new object[] {handlingContext});
            }

            private static void SignalThatTheTreadIsComplete(ICommandHandlingContext<ICommand> handlingContext)
            {
                ((ICommandHandlingContext) handlingContext).WaitHandle.Set();
            }

            private static void WaitForTheThreadToComplete(ICommandHandlingContext<ICommand> handlingContext)
            {
                ((ICommandHandlingContext) handlingContext).WaitHandle.WaitOne();
            }

            private ICommandHandlingContext<ICommand> CreateTheCommandHandlingContext(ICommand command)
            {
                var handlingContextType = typeof (CommandHandlingContext<>).MakeGenericType(commandType);
                return (ICommandHandlingContext<ICommand>) Activator.CreateInstance(handlingContextType, command);
            }

            private object CreateTheCommandHandler()
            {
                return serviceLocator.Resolve(commandHandlerType);
            }

            private MethodInfo GetTheHandleMethod()
            {
                return typeof (IHandleCommands<>).MakeGenericType(commandType).GetMethod("Handle");
            }
        }

        private interface ICommandHandlingContext
        {
            ICommand Command { get; }
            int ReturnValue { get; }
            ManualResetEvent WaitHandle { get; }
        }

        private class CommandHandlingContext<TCommand> : ICommandHandlingContext, ICommandHandlingContext<TCommand>
            where TCommand : ICommand
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