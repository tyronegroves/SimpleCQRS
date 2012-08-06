using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
            try
            {
                var events = GetTheDomainEvents();
                foreach (var @event in events)
                {
                    var eventId = @event.EventId;
                    db.EventStore.DeleteByEventId(eventId);
                }
            } catch
            {
            }

            db = null;
        }

        [Given(@"the EventStore table does not exist")]
        public void GivenTheEventStoreTableDoesNotExist()
        {
            var sqlConfiguration = ScenarioContext.Current.Get<SqlServerConfiguration>();

            var createSql =
                @"
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{1}]') AND type in (N'U'))
begin
Drop table [{1}];
end";
            using (var connection = new SqlConnection(sqlConfiguration.ConnectionString))
            {
                connection.Open();
                var sql = string.Format(createSql, "", "EventStore");
                using (var command = new SqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }


        [Given(@"I have the following events in the database")]
        public void GivenIHaveTheFollowingEventsInTheDatabase(Table table)
        {
            dynamic db = GetTheDatabase();

            var eventStoreRecords = table.CreateSet<EventStore>();

            foreach (var record in eventStoreRecords)
            {
                dynamic recordToAdd = new SimpleRecord();
                recordToAdd.Data = record.Data;
                recordToAdd.Sequence = record.Sequence;
                recordToAdd.EventDate = record.EventDate;
                recordToAdd.AggregateRootId = record.AggregateRootId;
                recordToAdd.EventType = record.EventType;
                db.EventStore.Insert(recordToAdd);
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
                                                   }).ToList();
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