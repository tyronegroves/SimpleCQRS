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
            var domainEvent = eventsToAdd.ToList().Last();
            domainEventSerializer.Setup(x => x.Serialize(domainEvent))
                .Returns(result);
        }

        [Given(@"deserializing '(.*)' will return a SomethingHappenedEvent with the following data")]
        public void z(string data, Table table)
        {
            domainEventSerializer.Setup(x => x.Deserialize(typeof (SomethingHappenedEvent), data))
                .Returns(table.CreateInstance<SomethingHappenedEvent>());
        }

        [Given(@"deserializing '(.*)' will return a SomethingElseHappenedEvent with the following data")]
        public void a(string data, Table table)
        {
            domainEventSerializer.Setup(x => x.Deserialize(typeof (SomethingElseHappenedEvent), data))
                .Returns(table.CreateInstance<SomethingElseHappenedEvent>());
        }

        [Then(@"I should get back the following DomainEvents")]
        public void b(Table table)
        {
            var domainEvents = ScenarioContext.Current.Get<IEnumerable<DomainEvent>>();
            table.CompareToSet(domainEvents);
        }
    }
}