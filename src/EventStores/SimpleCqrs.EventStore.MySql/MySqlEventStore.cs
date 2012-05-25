using System;
using System.Collections.Generic;
// using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Text;
using ServiceStack.Text;
using SimpleCqrs.Eventing;
using Dapper;

namespace SimpleCqrs.EventStore.MySql
{
    public class MySqlEventStore : IEventStore
    {
        private readonly IDomainEventSerializer serializer;
        private readonly MySqlServerConfiguration configuration;

        public MySqlEventStore(MySqlServerConfiguration configuration, IDomainEventSerializer serializer)
        {
            this.serializer = serializer;
            this.configuration = configuration;
            Init();
        }

        public void Init()
        {
            using (var connection = new MySqlConnection(configuration.ConnectionString))
            {
                connection.Open();
                var sql = string.Format(MySqlStatements.CreateTheEventStoreTable, "EventStore");
                using (var command = new MySqlCommand(sql, connection))
                    command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public IEnumerable<DomainEvent> GetEvents(Guid aggregateRootId, int startSequence)
        {
            var events = new List<DomainEvent>();
            using (var connection = new MySqlConnection(configuration.ConnectionString))
            {
                connection.Open();
                var sql = string.Format(MySqlStatements.GetEventsByAggregateRootAndSequence, "", "EventStore", aggregateRootId,
                                        startSequence);
                using (var command = new MySqlCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                    while (reader.Read())
                    {
                        var type = reader["EventType"].ToString();
                        var data = reader["data"].ToString();

                        try
                        {
                            events.Add(serializer.Deserialize(Type.GetType(type), data));
                        } catch(ArgumentNullException ex) 
                        {
                            throw new Exception(string.Format("Cannot find type '{0}', yet the type is in the event store. Are you sure you haven't changed a class name or something arising from mental dullness?", type.Split(',')[0]), ex.InnerException);
                        }
                    }
                connection.Close();
            }
            return events;
        }

        public void Insert(IEnumerable<DomainEvent> domainEvents)
        {
            var queries = domainEvents.Select( domainEvent => 
                new {
                    EventType = TypeToStringHelperMethods.GetString(domainEvent.GetType()), 
                    AggregateRootId = domainEvent.AggregateRootId,  
                    EventDate = domainEvent.EventDate, 
                    Sequence = domainEvent.Sequence, 
                    Data = (serializer.Serialize(domainEvent) ?? string.Empty)
                                 .Replace("'", "''")
                });


            if (!queries.Any()) return;

            using (var connection = new MySqlConnection(configuration.ConnectionString))
            {
                connection.Open();
                connection.Execute(string.Format(MySqlStatements.InsertEvents, "EventStore"), queries);
                connection.Close();
            }
        }

        public IEnumerable<DomainEvent> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes)
        {
            var events = new List<DomainEvent>();

            var eventParameters = domainEventTypes.Select(TypeToStringHelperMethods.GetString).Join("','");

            using (var connection = new MySqlConnection(configuration.ConnectionString))
            {
                connection.Open();
                var sql = string.Format(MySqlStatements.GetEventsByType, "EventStore", eventParameters);
                using (var command = new MySqlCommand(sql, connection))
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

        public IEnumerable<DomainEvent> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes, Guid aggregateRootId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DomainEvent> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }
    }
}