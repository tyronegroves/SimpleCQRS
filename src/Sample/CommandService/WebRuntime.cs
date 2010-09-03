using SimpleCqrs;
using SimpleCqrs.Eventing;
using SimpleCqrs.EventStore.MongoDb;
using SimpleCqrs.Unity;

namespace Commands
{
    public class WebRuntime : SimpleCqrsRuntime<UnityServiceLocator>
    {
        protected override IEventStore GetEventStore(IServiceLocator serviceLocator)
        {
            return new MongoEventStore("Server=192.168.2.2", serviceLocator.Resolve<ITypeCatalog>());
        }
    }
}