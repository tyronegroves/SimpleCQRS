using System.Collections.Generic;
using SimpleCqrs.Eventing;
using SimpleCqrs.EventStore.SqlServer.Serializers;
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

        private static SqlServerEventStore CreateTheEventStore()
        {
            var sqlServerConfiguration = ScenarioContext.Current.Get<SqlServerConfiguration>();
            var domainEventSerializer = ScenarioContext.Current.Get<IDomainEventSerializer>();
            return new SqlServerEventStore(sqlServerConfiguration, domainEventSerializer);
        }
    }
}