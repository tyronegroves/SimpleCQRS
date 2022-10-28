namespace EventSourcingCQRS.Eventing
{
    public class DomainEventHandlerFactory : IDomainEventHandlerFactory
    {
        private readonly IServiceLocator serviceLocator;

        public DomainEventHandlerFactory(IServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
        }

        public object Create(Type domainEventHandlerType)
        {
            return serviceLocator.Resolve(domainEventHandlerType);
        }
    }
}