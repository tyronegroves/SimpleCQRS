using System.Collections.Generic;
using SimpleCqrs.Eventing;
using TechTalk.SpecFlow;

namespace SimpleCqrs.EventStore.SqlServer.Tests.Specs
{
    [Binding]
    public class SqlServerEventStoreSteps
    {
        [When(@"I add the domain events to the store")]
        public void WhenIAddTheDomainEventsToTheStore()
        {
            var eventsToAdd = ScenarioContext.Current.Get<IEnumerable<DomainEvent>>();

            var eventStore = CreateTheEventStore();
            eventStore.Insert(eventsToAdd);
        }

        private SqlServerEventStore CreateTheEventStore()
        {
            return ScenarioContext.Current.Get<IServiceLocator>().Resolve<SqlServerEventStore>();
        }
    }
}