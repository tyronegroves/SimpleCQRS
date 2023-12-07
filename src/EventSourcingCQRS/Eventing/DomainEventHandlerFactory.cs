using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace EventSourcingCQRS.Eventing
{
    [ExcludeFromCodeCoverage]
    public class DomainEventHandlerFactory : IDomainEventHandlerFactory
    {
        private readonly IServiceProvider _serviceLocator;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public DomainEventHandlerFactory(IServiceProvider serviceLocator, IServiceScopeFactory serviceScopeFactory)
        {
            _serviceLocator = serviceLocator;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public object Create(Type domainEventHandlerType)
        {
            //return _serviceLocator.GetService(domainEventHandlerType);

            try
            {
                var eventHandler = _serviceLocator.GetService(domainEventHandlerType);

                if (eventHandler == null)
                {
                    eventHandler = ActivatorUtilities.CreateInstance(_serviceLocator, domainEventHandlerType);
                }

                return eventHandler;
            }
            catch
            {
                using var scope = _serviceScopeFactory.CreateScope();
                return scope.ServiceProvider.GetService(domainEventHandlerType);
            }
        }
    }
}