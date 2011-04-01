using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ServiceStack.Text;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.EventStore.SqlServer
{
    public class SqlServerEventStore : IEventStore
    {
        private readonly IDomainEventSerializer serializer;
        private readonly SqlServerConfiguration configuration;

        public SqlServerEventStore(string connectionString, IDomainEventSerializer serializer) :
            this(new SqlServerConfiguration(connectionString), serializer)
        {
        }

        public SqlServerEventStore(SqlServerConfiguration configuration, IDomainEventSerializer serializer)
        {
            this.serializer = serializer;
            this.configuration = configuration;
            Init();
        }

        public void Init()
        {
            using (var connection = new SqlConnection(configuration.ConnectionString))
            {
                connection.Open();
                var sql = string.Format(SqlStatements.CreateTheEventStoreTable, "EventStore");
                using (var command = new SqlCommand(sql, connection))
                    command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public IEnumerable<DomainEvent> GetEvents(Guid aggregateRootId, int startSequence)
        {
            var events = new List<DomainEvent>();
            using (var connection = new SqlConnection(configuration.ConnectionString))
            {
                connection.Open();
                var sql = string.Format(SqlStatements.GetEventsByAggregateRootAndSequence, "", "EventStore", aggregateRootId,
                                        startSequence);
                using (var command = new SqlCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                    while (reader.Read())
                    {
                        var type = reader["EventType"].ToString();
                        var data = reader["data"].ToString();

                        events.Add(serializer.Deserialize(Type.GetType(type), data));
                    }
                connection.Close();
            }
            return events;
        }

        public void Insert(IEnumerable<DomainEvent> domainEvents)
        {
            var sql = new StringBuilder();
            foreach (var de in domainEvents)
                sql.AppendFormat(SqlStatements.InsertEvents, "EventStore", GetTheType(de), de.AggregateRootId, de.EventDate, de.Sequence,
                                 serializer.Serialize(de));

            if (sql.Length <= 0) return;

            using (var connection = new SqlConnection(configuration.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(sql.ToString(), connection))
                    command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public IEnumerable<DomainEvent> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes)
        {
            var events = new List<DomainEvent>();

            var eventParameters = domainEventTypes.Select(TypeToStringHelperMethods.GetString).Join("','");

            using (var connection = new SqlConnection(configuration.ConnectionString))
            {
                connection.Open();
                var sql = string.Format(SqlStatements.GetEventsByType, "EventStore", eventParameters);
                using (var command = new SqlCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                    while (reader.Read())
                    {
                        var type = reader["EventType"].ToString();
                        var data = reader["data"].ToString();

                        var domainEvent = serializer.Deserialize(Type.GetType(type), data);
                        events.Add(domainEvent);
                    }
                connection.Close();
            }
            return events;
        }

        private static string GetTheType(DomainEvent domainEvent)
        {
            return TypeToStringHelperMethods.GetString(domainEvent.GetType());
        }
    }
}