using SimpleCqrs.Domain;

namespace SimpleCqrs.Commanding
{
    public abstract class AggregateRootCommandHandler<TCommand, TAggregateRoot> : IHandleCommands<TCommand>
        where TCommand : ICommandWithAggregateRootId
        where TAggregateRoot : AggregateRoot, new()
    {
        private readonly IDomainRepository domainRepository;

        protected AggregateRootCommandHandler() : this(ServiceLocator.Current.Resolve<IDomainRepository>())
        {
        }

        protected AggregateRootCommandHandler(IDomainRepository domainRepository)
        {
            this.domainRepository = domainRepository;
        }

        void IHandleCommands<TCommand>.Handle(ICommandHandlingContext<TCommand> handlingContext)
        {
            var command = handlingContext.Command;

            var aggregateRoot = domainRepository.GetById<TAggregateRoot>(command.AggregateRootId);

            ValidateTheCommand(handlingContext, command, aggregateRoot);

            Handle(command, aggregateRoot);

            if(aggregateRoot != null)
                domainRepository.Save(aggregateRoot);
        }

        private void ValidateTheCommand(ICommandHandlingContext<TCommand> handlingContext, TCommand command, TAggregateRoot aggregateRoot)
        {
            ValidationResult = ValidateCommand(command, aggregateRoot);
            handlingContext.Return(ValidationResult);
        }

        protected int ValidationResult { get; private set; }

        public virtual int ValidateCommand(TCommand command, TAggregateRoot aggregateRoot)
        {
            return 0;
        }

        public abstract void Handle(TCommand command, TAggregateRoot aggregateRoot);
    }
}