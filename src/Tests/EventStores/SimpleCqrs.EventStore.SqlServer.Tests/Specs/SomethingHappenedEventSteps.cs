using System;
using System.Collections.Generic;
using Moq;
using SimpleCqrs.Eventing;
using SimpleCqrs.Unity;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SimpleCqrs.EventStore.SqlServer.Tests.Specs
{
    [Binding]
    public class RuntimeSteps
    {
        public static TestingRuntime Runtime;

        [BeforeTestRun]
        public static void TestRun()
        {
            Runtime = new TestingRuntime();
            Runtime.Start();
        }

        [AfterTestRun]
        public static void Cleanup()
        {
            Runtime.Shutdown();
        }

        [BeforeScenario]
        public void Setup()
        {
            ScenarioContext.Current.Set<IServiceLocator>(Runtime.ServiceLocator);
        }

        [AfterScenario]
        public void Teardown()
        {
        }
    }

    public class TestingRuntime : SimpleCqrsRuntime<UnityServiceLocator>
    {
    }

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

    public class SqlServerEventStore : IEventStore
    {
        public IEnumerable<DomainEvent> GetEvents(Guid aggregateRootId, int startSequence)
        {
            throw new NotImplementedException();
        }

        public void Insert(IEnumerable<DomainEvent> domainEvents)
        {
            
        }

        public IEnumerable<DomainEvent> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes)
        {
            throw new NotImplementedException();
        }
    }

    [Binding]
    public class SqlStatementRunner
    {
        private Mock<ISqlStatementRunner> fakeSqlStatementRunner;

        [BeforeScenario]
        public void Setup()
        {
            fakeSqlStatementRunner = new Mock<ISqlStatementRunner>();
            RuntimeSteps.Runtime.ServiceLocator.Register(fakeSqlStatementRunner.Object);
        }

        [Then(@"the following SQL statement should be run")]
        public void x(string expectedSqlStatement)
        {
            fakeSqlStatementRunner.Verify(x=>x.RunThisSql(expectedSqlStatement), Times.Once());
        }
    }

    public interface ISqlStatementRunner
    {
        void RunThisSql(string sqlStatement);
    }

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

    public class SomethingHappenedEvent : DomainEvent
    {
    }
}