using System;
using System.Collections.Generic;
using NServiceBus;
using SimpleCqrs.Commanding;

namespace SimpleCqrs.NServiceBus.Commanding
{
    public class NsbCommandBus : ICommandBus
    {
        private readonly IDictionary<Type, string> commandTypeToDestinationLookup;
        private readonly TimeSpan executeTimeout;

        public NsbCommandBus(IBus bus, IDictionary<Type, string> commandTypeToDestinationLookup, TimeSpan executeTimeout)
        {
            this.commandTypeToDestinationLookup = commandTypeToDestinationLookup;
            this.executeTimeout = executeTimeout;
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

            if(!asyncResult.AsyncWaitHandle.WaitOne(executeTimeout))
                throw new ExecuteTimeoutException();

            return ((CompletionResult)asyncResult.AsyncState).ErrorCode;
        }

        public void Send<TCommand>(TCommand command) where TCommand : ICommand
        {
            var destination = commandTypeToDestinationLookup[typeof(TCommand)];
            InnerBus.Send<CommandMessage>(destination, message => message.Command = command);
        }
    }
}