using Microsoft.Extensions.DependencyInjection;

namespace EventSourcingCQRS.Commanding
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandDispatcher(IServiceProvider serviceProvider) => this._serviceProvider = serviceProvider;

        public Task<TCommandResult> Dispatch<TCommand, TCommandResult>(TCommand command, CancellationToken cancellation)
        {
            var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand, TCommandResult>>();
            return handler.Handle(command, cancellation);
        }
    }
}  