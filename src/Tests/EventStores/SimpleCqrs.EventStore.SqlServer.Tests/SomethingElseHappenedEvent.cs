using SimpleCqrs.Eventing;

namespace SimpleCqrs.EventStore.SqlServer.Tests
{
    public class SomethingElseHappenedEvent : DomainEvent
    {
        public string SomeDataToStore { get; set; }
    }
}