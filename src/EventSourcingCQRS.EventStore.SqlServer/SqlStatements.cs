namespace EventSourcingCQRS.EventStore.SqlServer
{
    public class SqlStatements
    {
        internal const string GetEventsByType = "select eventtype, data from {0} where eventtype in ('{1}')";
        internal const string InsertEvents = "insert into {0} values ('{1}', '{2}', '{3}', {4}, '{5}')";
        internal const string GetEventsByAggregateRootAndSequence = "select eventtype, data from {1} where aggregaterootid = '{2}' and sequence >= {3}";
        internal const string CreateTheEventStoreTable = @"IF  not EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{0}]') AND type in (N'U'))
begin
create table dbo.[{0}](
   EventId bigint  identity not null primary key,
   EventType nvarchar(255) not null,
   AggregateRootId uniqueidentifier not null,
   EventDate datetimeoffset not null,
   Sequence int not null,
   Data nvarchar(max) not null
)
end";
    }
}