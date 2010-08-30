using SimpleCqrs;
using SimpleCqrs.Eventing;
using SimpleCqrs.EventStore.MongoDb;
using SimpleCqrs.Unity;

namespace Commands
{
    public class WebBootstrapper : Bootstrapper<UnityServiceLocator>
    {
        protected override IEventStore GetEventStore(IServiceLocator serviceLocator)
        {
            return new MongoEventStore("Server=localhost", serviceLocator.Resolve<ITypeCatalog>());
        }
    }
}