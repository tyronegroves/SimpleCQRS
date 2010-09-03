using SimpleCqrs.Domain;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.Commanding
{
    public abstract class AggregateRootCommandHandler<TCommand, TAggregateRoot> : IHandleCommands<TCommand>
        where TCommand : ICommandWithAggregateRootId
        where TAggregateRoot : AggregateRoot, new()
    {
        private IDomainRepository domainRepository;
        private ICommandHandlingContext<TCommand> context;

        void IHandleCommands<TCommand>.Handle(ICommandHandlingContext<TCommand> handlingContext)
        {
            var serviceLocator = ServiceLocator.Current;
            domainRepository = serviceLocator.Resolve<IDomainRepository>();
            context = handlingContext;
            var aggregateRoot = domainRepository.GetById<TAggregateRoot>(context.Command.AggregateRootId);
            Handle(handlingContext.Command, aggregateRoot);
        }

        protected abstract void Handle(TCommand command, TAggregateRoot aggregateRoot);

        protected void Return(int value)
        {
            context.Return(value);
        }
    }
}