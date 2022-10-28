using System.Reflection;

namespace EventSourcingCQRS.Commanding
{
    public class LocalCommandBus : ICommandBus, IHaveATestMode
    {
        private readonly IDictionary<Type, CommandHandlerInvoker> commandInvokers;

        public LocalCommandBus(ITypeCatalog typeCatalog, IServiceLocator serviceLocator)
        {
            commandInvokers = CommandInvokerDictionaryBuilderHelpers.CreateADictionaryOfCommandInvokers(typeCatalog, serviceLocator);
        }

        bool IHaveATestMode.IsInTestMode { get; set; }

        public int Execute<TCommand>(TCommand command) where TCommand : ICommand
        {
            var commandHandler = GetTheCommandHandler(command);
            return commandHandler == null ? 0 : commandHandler.Execute(command);
        }

        public void Send<TCommand>(TCommand command) where TCommand : ICommand
        {
            var commandHandler = GetTheCommandHandler(command);
            
            if (commandHandler == null) return;
            
            commandHandler.Send(command);
        }

        private CommandHandlerInvoker GetTheCommandHandler(ICommand command)
        {
            CommandHandlerInvoker commandInvoker;
            if(!commandInvokers.TryGetValue(command.GetType(), out commandInvoker) && !((IHaveATestMode)this).IsInTestMode)
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

                try
                {
                    handleMethod.Invoke(commandHandler, new object[] { handlingContext });
                }
                catch(TargetInvocationException ex)
                {
                    throw new Exception(
                        string.Format("Command handler '{0}' for '{1}' failed. Inspect inner exception.", commandHandler.GetType().Name, handlingContext.Command.GetType().Name),
                        ex.InnerException);
                }
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