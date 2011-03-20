using System.Collections.Generic;
using SimpleCqrs.Eventing;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SimpleCqrs.EventStore.SqlServer.Tests.Specs
{
    [Binding]
    public class DomainEventSteps
    {
        private IList<DomainEvent> eventsToAdd;

        [BeforeScenario]
        public void Setup()
        {
            eventsToAdd = new List<DomainEvent>();
            ScenarioContext.Current.Set<IEnumerable<DomainEvent>>(() => eventsToAdd);
        }

        [Given(@"I have a SomethingHappenedEvent to be added to the store with the following values")]
        public void x(Table table)
        {
            var somethingHappenedEvent = table.CreateInstance<SomethingHappenedEvent>();
            eventsToAdd.Add(somethingHappenedEvent);
        }
    }
}