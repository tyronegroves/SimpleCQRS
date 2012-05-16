namespace SimpleCqrs.EventStore.SqlServer
{
    public class SqlStatements
    {
        internal const string xGetEventsByType = "select eventtype, data from {0} where eventtype in ('{1}')";
        internal const string xInsertEvents = "insert into {0} values ('{1}', '{2}', Convert(datetime,'{3}'), {4}, '{5}')";
        internal const string xGetEventsByAggregateRootAndSequence = "select eventtype, data from {1} where aggregaterootid = '{2}' and sequence >= {3}";
        internal const string xCreateTheEventStoreTable = @"IF  not EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{0}]') AND type in (N'U'))
begin
create table dbo.[{0}](
   EventId int identity not null primary key,
   EventType nvarchar(255),
   AggregateRootId uniqueidentifier not null,
   EventDate datetime not null,
   Sequence int not null,
   Data nvarchar(max)
)
end";
        internal const string GetEventsByType = "SELECT EVENTTYPE, DATA FROM {0} WHERE EVENTTYPE IN ('{1}')";
        internal const string InsertEvents = "";
        internal const string GetEventsByAggregateRootAndSequence = "";
        internal const string CreateTheEventStoreTable = @"CREATE TABLE IF NOT EXISTS {0}
(
    EventId INT NOT NULL,
    EventType  NVARCHAR(255),
    AggregateRootId UNIQUEIDENTIFIER NOT NULL,
    EventDate DATETIME NOT NULL, 
    Sequence int NOT NULL,
    Data BLOB,
    CONSTRAINT PK_{0} PRIMARY KEY (EventId),
    CONSTRAINT UNIQUE INDEX IX_{0}_AggregateRootId ON {0} (AggregateRootId)
);


";
    }
}