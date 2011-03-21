using System;
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

        [When(@"I retrieve the domain events for '(.*)'")]
        public void z(string aggregateRootId)
        {
            var eventStore = CreateTheEventStore();
            var events = eventStore.GetEvents(new Guid(aggregateRootId), 0);

            ScenarioContext.Current.Set(events);
        }

        private static SqlServerEventStore CreateTheEventStore()
        {
            var sqlServerConfiguration = ScenarioContext.Current.Get<SqlServerConfiguration>();
            var domainEventSerializer = ScenarioContext.Current.Get<IDomainEventSerializer>();
            return new SqlServerEventStore(sqlServerConfiguration, domainEventSerializer);
        }
    }
}