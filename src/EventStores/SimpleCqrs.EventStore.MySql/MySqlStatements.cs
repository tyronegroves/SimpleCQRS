namespace SimpleCqrs.EventStore.MySql
{
    public class MySqlStatements
    {
        internal const string GetEventsByType = "SELECT EVENTTYPE, DATA FROM {0} WHERE EVENTTYPE IN ('{1}')";
        internal const string InsertEvents = "INSERT INTO {0} VALUES ('{1}', '{2}', '{3}', '{4}', '{5}')";
        internal const string GetEventsByAggregateRootAndSequence = "SELECT eventtype, data FROM {1} WHERE aggregaterootid = '{2}' AND sequence >= {3}";
        internal const string CreateTheEventStoreTable = @"CREATE TABLE IF NOT EXISTS {0}
(
    EventId INT NOT NULL,
    EventType  NVARCHAR(255),
    AggregateRootId VARBINARY(36) NOT NULL,
    EventDate DATETIME NOT NULL, 
    Sequence int NOT NULL,
    Data BLOB,
    CONSTRAINT PK_{0} PRIMARY KEY (EventId),
    CONSTRAINT UNIQUE INDEX IX_{0}_AggregateRootId USING (AggregateRootId)
);";
    }
}