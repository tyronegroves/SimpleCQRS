using System;
using System.Collections.Generic;
using System.Linq;
using Simple.Data;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SimpleCqrs.EventStore.SqlServer.Tests.Steps
{
    [Binding]
    public class DatabaseSteps
    {
        public static void ClearTheEventStore()
        {
            dynamic db = GetTheDatabase();

            var events = GetTheDomainEvents();
            foreach (var @event in events)
            {
                var eventId = @event.EventId;
                db.EventStore.DeleteByEventId(eventId);
            }
        }

        [Then(@"I should have the following events in the database")]
        public void x(Table table)
        {
            var events = GetTheDomainEvents();

            table.CompareToSet(events);
        }

        private static IEnumerable<EventStore> GetTheDomainEvents()
        {
            dynamic db = GetTheDatabase();

            IEnumerable<dynamic> databaseRecords = db.EventStore.FindAll(db.EventStore.EventType != "x").ToList()
                                                   ?? new dynamic[] {};

            return databaseRecords.Select(x => new EventStore
                                                   {
                                                       AggregateRootId = x.AggregateRootId,
                                                       Data = x.Data,
                                                       EventDate = x.EventDate,
                                                       EventId = x.EventId,
                                                       EventType = x.EventType,
                                                       Sequence = x.Sequence
                                                   });
        }

        private static object GetTheDatabase()
        {
            var sqlServerConfiguration = ScenarioContext.Current.Get<SqlServerConfiguration>();

            return Database.OpenConnection(sqlServerConfiguration.ConnectionString);
        }
    }

    public class EventStore
    {
        public int EventId { get; set; }
        public string EventType { get; set; }
        public Guid AggregateRootId { get; set; }
        public DateTime EventDate { get; set; }
        public int Sequence { get; set; }
        public string Data { get; set; }
    }
}