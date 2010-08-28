using SimpleCqrs;
using SimpleCqrs.Events;
using SimpleCqrs.EventStore.MongoDb;
using SimpleCqrs.Unity;

namespace Web
{
    public class WebBootstrapper : Bootstrapper<UnityServiceLocator>
    {
        protected override IEventStore GetEventStore(IServiceLocator serviceLocator)
        {
            return new MongoEventStore("Server=localhost", serviceLocator.Resolve<ITypeCatalog>());
        }
    }
}