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
            const string createSql = @"
IF  not EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{0}]') AND type in (N'U'))
begin
create table [{0}](
   EventId int identity not null primary key,
   EventType nvarchar(255),
   AggregateRootId uniqueidentifier not null,
   EventDate datetime not null,
   Sequence int not null,
   Data nvarchar(max)
)
end";
            using (var connection = new SqlConnection(configuration.ConnectionString))
            {
                connection.Open();
                var sql = string.Format(createSql, "EventStore");
                using (var command = new SqlCommand(sql, connection))
                    command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public IEnumerable<DomainEvent> GetEvents(Guid aggregateRootId, int startSequence)
        {
            var events = new List<DomainEvent>();
            const string fetchSql = "select eventtype, data from {1} where aggregaterootid = '{2}' and sequence >= {3}";
            using (var connection = new SqlConnection(configuration.ConnectionString))
            {
                connection.Open();
                var sql = string.Format(fetchSql, "", "EventStore", aggregateRootId,
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
            {
                var type = GetTheType(de);

                sql.AppendFormat("insert into {0} values ('{1}', '{2}', '{3}', {4}, '{5}')",
                                 "EventStore",
                                 type, de.AggregateRootId, de.EventDate, de.Sequence,
                                 serializer.Serialize(de));
            }

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

            const string fetchSql = "select eventtype, data from {0} where eventtype in ('{1}')";
            using (var connection = new SqlConnection(configuration.ConnectionString))
            {
                connection.Open();
                var sql = string.Format(fetchSql, "EventStore", eventParameters);
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