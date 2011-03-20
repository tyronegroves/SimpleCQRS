using System.Collections.Generic;
using System.Linq;
using Moq;
using SimpleCqrs.Eventing;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SimpleCqrs.EventStore.SqlServer.Tests.Specs
{
    [Binding]
    public class DomainEventSteps
    {
        private IList<DomainEvent> eventsToAdd;
        private Mock<IDomainEventSerializer> domainEventSerializer;

        [BeforeScenario]
        public void Setup()
        {
            domainEventSerializer = new Mock<IDomainEventSerializer>();
            eventsToAdd = new List<DomainEvent>();

            ScenarioContext.Current.Set<IEnumerable<DomainEvent>>(() => eventsToAdd);
            ScenarioContext.Current.Set(domainEventSerializer.Object);
        }

        [Given(@"I have a SomethingHappenedEvent to be added to the store with the following values")]
        public void x(Table table)
        {
            var somethingHappenedEvent = table.CreateInstance<SomethingHappenedEvent>();
            eventsToAdd.Add(somethingHappenedEvent);
        }

        [Given(@"I have a SomethingElseHappenedEvent to be added to the store with the following values")]
        public void y(Table table)
        {
            var somethingHappenedEvent = table.CreateInstance<SomethingElseHappenedEvent>();
            eventsToAdd.Add(somethingHappenedEvent);
        }

        [Given(@"that event will serialize to '(.*)'")]
        public void x(string result)
        {
            domainEventSerializer.Setup(x=>x.Serialize(eventsToAdd.Last()))
                .Returns(result);
        }
    }
}