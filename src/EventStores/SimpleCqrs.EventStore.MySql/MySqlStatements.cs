namespace SimpleCqrs.EventStore.MySql
{
    public class MySqlStatements
    {
        internal const string GetEventsByType = "SELECT EVENTTYPE, DATA FROM {0} WHERE EVENTTYPE IN (@EventType)";
        internal const string InsertEvents = @"INSERT INTO {0} (EventType, AggregateRootId, EventDate, Sequence, Data) VALUES ( @EventType, @AggregateRootId, @EventDate, @Sequence, @Data)";
        internal const string GetEventsByAggregateRootAndSequence = @"SELECT eventtype, data FROM {1} WHERE aggregaterootid = @AggregateRootId AND sequence >= @Sequence";
        internal const string CreateTheEventStoreTable = @"CREATE TABLE IF NOT EXISTS {0}
(
    EventId INT AUTO_INCREMENT NOT NULL,
    EventType VARCHAR(255),
    AggregateRootId VARBINARY(36) NOT NULL,
    EventDate DATETIME NOT NULL, 
    Sequence int NOT NULL,
    Data BLOB,
    CONSTRAINT PK_{0} PRIMARY KEY (EventId)
);";

        
    }
}