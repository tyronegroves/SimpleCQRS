using SimpleCqrs.Domain;

namespace SimpleCqrs.Commanding
{
    public abstract class AggregateRootCommandHandler<TCommand, TAggregateRoot> : IHandleCommands<TCommand>
        where TCommand : ICommandWithAggregateRootId
        where TAggregateRoot : AggregateRoot, new()
    {
        void IHandleCommands<TCommand>.Handle(ICommandHandlingContext<TCommand> handlingContext)
        {
            var command = handlingContext.Command;

            ValidateTheCommand(handlingContext, command);

            var domainRepository = GetTheDomainRepository();
            var aggregateRoot = domainRepository.GetById<TAggregateRoot>(command.AggregateRootId);

            Handle(command, aggregateRoot);
            domainRepository.Save(aggregateRoot);
        }

        private static IDomainRepository GetTheDomainRepository()
        {
            return ServiceLocator.Current.Resolve<IDomainRepository>();
        }

        private void ValidateTheCommand(ICommandHandlingContext<TCommand> handlingContext, TCommand command)
        {
            ValidationResult = ValidateCommand(command);
            handlingContext.Return(ValidationResult);
        }

        protected int ValidationResult { get; private set; }

        public virtual int ValidateCommand(TCommand command)
        {
            return 0;
        }

        public abstract void Handle(TCommand command, TAggregateRoot aggregateRoot);
    }
}