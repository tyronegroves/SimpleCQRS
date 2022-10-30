using Microsoft.Extensions.DependencyInjection;

namespace EventSourcingCQRS.Commanding
{
    internal class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider serviceProvider;

        public CommandDispatcher(IServiceProvider serviceProvider) => this.serviceProvider = serviceProvider;

        public Task<TCommandResult> Dispatch<TCommand, TCommandResult>(TCommand command, CancellationToken cancellation)
        {
            var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand, TCommandResult>>();
            return handler.Handle(command, cancellation);
        }
    }
}