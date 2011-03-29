﻿using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleCqrs.Eventing;
using System.Reflection;
using System.Data.SqlClient;

using ServiceStack.Text;

namespace SimpleCqrs.EventStore.SqlServer
{
    public class SqlServerEventStore : IEventStore
    {
        private IDomainEventSerializer serializer = null;
        private SqlServerConfiguration configuration = null;

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
            var createSql =
                @"
IF  not EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{1}]') AND type in (N'U'))
begin
create table [{1}](
   EventId int identity not null primary key,
   EventType nvarchar(255),
   AggregateRootId uniqueidentifier not null,
   EventDate datetime not null,
   Sequence int not null,
   Serialized nvarchar(max)
)
end";
            using (var connection = new SqlConnection(configuration.ConnectionString))
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

        public IEnumerable<DomainEvent> GetEvents(Guid aggregateRootId, int startSequence)
        {
            var events = new List<DomainEvent>();
            var fetchSql = "select eventtype, data from {1} where aggregaterootid = '{2}' and sequence >= {3}";
            var sql = string.Format(fetchSql, "", "EventStore", aggregateRootId,
                                        startSequence);

            var runner = new SqlRunner();

            var dataRows = runner.ExecuteQuery(configuration, sql);

            foreach(DataRow row in dataRows)
            {
                string type = row["EventType"].ToString();
                string data = row["data"].ToString();

                events.Add(serializer.Deserialize(Type.GetType(type), data));
            }

            return events;
        }

        public void Insert(IEnumerable<DomainEvent> domainEvents)
        {
            var sql = new StringBuilder();
            foreach (var de in domainEvents)
            {
                sql.AppendFormat("insert into {0} values ('{1}', '{2}', '{3}', {4}, '{5}')",
                                 "EventStore",
                                 de.GetType().AssemblyQualifiedName, de.AggregateRootId, de.EventDate, de.Sequence,
                                 serializer.Serialize(de));
            }

            if (sql.Length > 0)
            {
                using (var connection = new SqlConnection(configuration.ConnectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(sql.ToString(), connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
        }

        public IEnumerable<DomainEvent> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes)
        {
            var events = new List<DomainEvent>();

            string eventParameters = domainEventTypes.Select(x=>x.AssemblyQualifiedName).Join("','");

            var fetchSql = "select eventtype, data from {0} where eventtype in ('{1}')";
            var sql = string.Format(fetchSql, "EventStore", eventParameters);

            var runner = new SqlRunner();

            var dataRows = runner.ExecuteQuery(configuration, sql);

            foreach (DataRow row in dataRows) {
                string type = row["EventType"].ToString();
                string data = row["data"].ToString();

                var domainEvent = serializer.Deserialize(Type.GetType(type), data);
                events.Add(domainEvent);
            }

            return events;
        }

    }
}