using SimpleCqrs.Domain;

namespace SimpleCqrs.Commanding
{
    public abstract class CreateCommandHandler<TCommand> : CommandHandler<TCommand> where TCommand : ICommand
    {
        public override void Handle(TCommand command)
        {
            var aggregateRoot = CreateAggregateRoot(command);

            Handle(command, aggregateRoot);

            var domainRepository = ServiceLocator.Current.Resolve<IDomainRepository>();

            domainRepository.Save(aggregateRoot);
        }

        public abstract AggregateRoot CreateAggregateRoot(TCommand command);

        public virtual void Handle(TCommand command, AggregateRoot aggregateRoot)
        {
        }
    }
}