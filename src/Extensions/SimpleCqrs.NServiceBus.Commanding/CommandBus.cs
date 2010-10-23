using System;
using System.Collections.Generic;
using NServiceBus;
using SimpleCqrs.Commanding;

namespace SimpleCqrs.NServiceBus.Commanding
{
    public class CommandBus : ICommandBus
    {
        private readonly IDictionary<Type, string> commandTypeToDestinationLookup;

        public CommandBus(IBus bus, IDictionary<Type, string> commandTypeToDestinationLookup)
        {
            this.commandTypeToDestinationLookup = commandTypeToDestinationLookup;
            InnerBus = bus;
        }

        public IBus InnerBus { get; private set; }

        public string GetDestinationForCommandType<TCommand>()
        {
            return commandTypeToDestinationLookup[typeof(TCommand)];
        }

        public int Execute<TCommand>(TCommand command) where TCommand : ICommand
        {
            var destination = GetDestinationForCommandType<TCommand>();
            var asyncResult = InnerBus
                .Send<CommandWithReturnValueMessage>(destination, message => message.Command = command)
                .Register(state => { }, null);

            asyncResult.AsyncWaitHandle.WaitOne();

            return ((CompletionResult)asyncResult.AsyncState).ErrorCode;
        }

        public void Send<TCommand>(TCommand command) where TCommand : ICommand
        {
            var destination = commandTypeToDestinationLookup[typeof(TCommand)];
            InnerBus.Send<CommandMessage>(destination, message => message.Command = command);
        }
    }
}