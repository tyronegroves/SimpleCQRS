//using ServiceStack.Text;

using System.Data.SqlClient;
using System.Text;
using EventSourcingCQRS.Eventing;

namespace EventSourcingCQRS.EventStore.SqlServer
{
    public class SqlServerEventStore : IEventStore
    {
        private readonly IDomainEventSerializer _serializer;
        private readonly SqlServerConfiguration _configuration;

        public SqlServerEventStore(SqlServerConfiguration configuration, IDomainEventSerializer serializer)
        {
            this._serializer = serializer;
            this._configuration = configuration;
            Init();
        }

        public void Init()
        {
            using var connection = new SqlConnection(_configuration.ConnectionString);
            connection.Open();
            var sql = string.Format(SqlStatements.CreateTheEventStoreTable, "EventStore");
            using (var command = new SqlCommand(sql, connection))
                command.ExecuteNonQuery();
            connection.Close();
        }



        public Task<IEnumerable<Guid>> GetDistinctAggregateRootsId()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<DomainEvent>> GetEvents(Guid aggregateRootId, int startSequence)
        {
            var events = new List<DomainEvent>();
            await using (var connection = new SqlConnection(_configuration.ConnectionString))
            {
                connection.Open();
                var sql = string.Format(SqlStatements.GetEventsByAggregateRootAndSequence, "", "EventStore", aggregateRootId,
                                        startSequence);
                await using (var command = new SqlCommand(sql, connection))
                await using (var reader = await command.ExecuteReaderAsync())
                    while (reader.Read())
                    {
                        var type = reader["EventType"].ToString();
                        var data = reader["data"].ToString();

                        try
                        {
                            events.Add(_serializer.Deserialize(Type.GetType(type), data));
                        }
                        catch (ArgumentNullException ex)
                        {
                            throw new Exception(
                                $"Cannot find type '{type.Split(',')[0]}', yet the type is in the event store. Are you sure you haven't changed a class name or something arising from mental dullness?", ex.InnerException);
                        }
                    }
                connection.Close();
            }
            return events;
        }

        public Task<IEnumerable<DomainEvent>> GetEventsUpToSequence(Guid aggregateRootId, int sequence)
        {
            throw new NotImplementedException();
        }

        public Task Insert(IEnumerable<DomainEvent> domainEvents)
        {
            var sql = new StringBuilder();
            foreach (var domainEvent in domainEvents)
                sql.AppendFormat(SqlStatements.InsertEvents, "EventStore", TypeToStringHelperMethods.GetString(domainEvent.GetType()), domainEvent.AggregateRootId, domainEvent.EventDate.ToUniversalTime().ToString("dd MMM yyyy HH:mm:ss"), domainEvent.Sequence,
                                 (_serializer.Serialize(domainEvent) ?? string.Empty)
                                 .Replace("'", "''"));

            if (sql.Length <= 0) return Task.CompletedTask;

            using (var connection = new SqlConnection(_configuration.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(sql.ToString(), connection))
                    command.ExecuteNonQuery();
                connection.Close();
            }

            return Task.CompletedTask;
        }

        public async Task<IEnumerable<DomainEvent>> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes)
        {
            var events = new List<DomainEvent>();

            var eventParameters = String.Join("','", domainEventTypes.Select(TypeToStringHelperMethods.GetString));

            await using var connection = new SqlConnection(_configuration.ConnectionString);
            connection.Open();
            var sql = string.Format(SqlStatements.GetEventsByType, "EventStore", eventParameters);
            await using (var command = new SqlCommand(sql, connection))
            await using (var reader = await command.ExecuteReaderAsync())
                while (reader.Read())
                {
                    var type = reader["EventType"].ToString();
                    var data = reader["data"].ToString();

                    var domainEvent = _serializer.Deserialize(Type.GetType(type), data);
                    events.Add(domainEvent);
                }
            connection.Close();
            return events;
        }

        public Task<IEnumerable<DomainEvent>> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes, Guid aggregateRootId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DomainEvent>> GetEventsByEventTypesUpToSequence(IEnumerable<Type> domainEventTypes, Guid aggregateRootId, int sequence)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DomainEvent>> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DomainEvent>> GetEventsByCriteria(Dictionary<string, string> criteria)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DomainEvent>> GetEventsByCriteria(List<EventQueryCriteria> criteria)
        {
            throw new NotImplementedException();
        }
    }
}