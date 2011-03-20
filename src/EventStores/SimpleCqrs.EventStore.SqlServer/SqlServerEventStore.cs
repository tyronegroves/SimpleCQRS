using System;
using System.Collections.Generic;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.EventStore.SqlServer
{
    public class SqlServerEventStore : IEventStore
    {
        private const string InsertStatement = @"Insert into Event_Store (EventType, AggregateRootId, EventDate, Sequence) 
Values ('{0}', '{1}', '{2}' ,'{3}');";
        private readonly ISqlStatementRunner sqlStatementRunner;

        public SqlServerEventStore(ISqlStatementRunner sqlStatementRunner)
        {
            this.sqlStatementRunner = sqlStatementRunner;
        }

        public IEnumerable<DomainEvent> GetEvents(Guid aggregateRootId, int startSequence)
        {
            throw new NotImplementedException();
        }

        public void Insert(IEnumerable<DomainEvent> domainEvents)
        {
            foreach (var domainEvent in domainEvents)
                sqlStatementRunner.RunThisSql(CreateSqlInsertStatement(domainEvent));
        }

        private static string CreateSqlInsertStatement(DomainEvent domainEvent)
        {
            return string.Format(InsertStatement, domainEvent.GetType().AssemblyQualifiedName,
                                 domainEvent.AggregateRootId.ToString().ToUpper(),
                                 domainEvent.EventDate,
                                 domainEvent.Sequence);
        }

        public IEnumerable<DomainEvent> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes)
        {
            throw new NotImplementedException();
        }
    }

//    public class SqlServerEventStore : IEventStore
//    {
//        private IDomainEventSerializer serializer = null;
//        private SqlServerConfiguration configuration = null;

//        public SqlServerEventStore(string connectionString, IDomainEventSerializer serializer) :
//            this(new SqlServerConfiguration(connectionString), serializer) {
//        }

//        public SqlServerEventStore(SqlServerConfiguration configuration, IDomainEventSerializer serializer) {
//            this.serializer = serializer;
//            this.configuration = configuration;
//            Init();
//        }

//        public void Init()
//        {
//            var createSql =
//                @"
//IF  not EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{0}].[{1}]') AND type in (N'U'))
//begin
//create table [{0}].[{1}](
//   EventId int identity not null primary key,
//   EventType nvarchar(255),
//   AggregateRootId uniqueidentifier not null,
//   EventDate datetime not null,
//   Sequence int not null,
//   Serialized nvarchar(max)
//)
//end";
//            using (var connection = new SqlConnection(configuration.ConnectionString))
//            {
//                connection.Open();
//                var sql = string.Format(createSql, configuration.Schema, configuration.TableName);
//                using(var command = new SqlCommand(sql,connection))
//                {
//                    command.ExecuteNonQuery();
//                }
//                connection.Close();
//            }
//        }

//        public IEnumerable<DomainEvent> GetEvents(Guid aggregateRootId, int startSequence)
//        {
//            var events = new List<DomainEvent>();
//            var fetchSql = "select eventtype, serialized from {0}.{1} where aggregaterootid = '{2}' and sequence >= {3}";
//            using (var connection = new SqlConnection(configuration.ConnectionString)) {
//                connection.Open();
//                var sql = string.Format(fetchSql, configuration.Schema, configuration.TableName, aggregateRootId,
//                                        startSequence);
//                using (var command = new SqlCommand(sql, connection)) {
//                    using(var reader = command.ExecuteReader())
//                    {
//                        while(reader.Read())
//                        {  
//                            string type = reader["EventType"].ToString();
//                            string serialized = reader["Serialized"].ToString();

//                            events.Add(serializer.Deserialize(Type.GetType(type), serialized));
//                        }
//                    }
//                }
//                connection.Close();
//            }
//            return events;
//        }

//        public void Insert(IEnumerable<DomainEvent> domainEvents)
//        {
//            var sql = new StringBuilder();
//            foreach (var de in domainEvents)
//            {
//                sql.AppendFormat("insert into {0}.{1} values ('{2}', '{3}', '{4}', {5}, '{6}')",
//                                 configuration.Schema, configuration.TableName,
//                                 de.GetType().AssemblyQualifiedName, de.AggregateRootId, de.EventDate, de.Sequence,
//                                 serializer.Serialize(de));
//            }

//            if (sql.Length > 0)
//            {
//                using (var connection = new SqlConnection(configuration.ConnectionString))
//                {
//                    connection.Open();
//                    using (var command = new SqlCommand(sql.ToString(), connection))
//                    {
//                        command.ExecuteNonQuery();
//                    }
//                    connection.Close();
//                }
//            }
//        }

//        public IEnumerable<DomainEvent> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes) {
//            var events = new List<DomainEvent>();

//            string eventParameters = domainEventTypes.Select(x => x.GetType().FullName).Join("','");

//            if(!string.IsNullOrWhiteSpace(eventParameters))
//            {
//                eventParameters = "'" + eventParameters + "'";
//            }

//            var fetchSql = "select eventtype, serialized from {0}.{1} where eventtype in '{2}'";
//            using (var connection = new SqlConnection(configuration.ConnectionString)) {
//                connection.Open();
//                var sql = string.Format(fetchSql, configuration.Schema, configuration.TableName, eventParameters);
//                using (var command = new SqlCommand(sql, connection)) {
//                    using (var reader = command.ExecuteReader()) {
//                        while (reader.Read()) {
//                            string type = reader["EventType"].ToString();
//                            string serialized = reader["Serialized"].ToString();

//                            events.Add(serializer.Deserialize(Type.GetType(type), serialized));
//                        }
//                    }
//                }
//                connection.Close();
//            }
//            return events;
//        }

//    }
}