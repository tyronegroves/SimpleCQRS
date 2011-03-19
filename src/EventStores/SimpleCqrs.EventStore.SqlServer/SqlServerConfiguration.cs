namespace SimpleCqrs.EventStore.SqlServer
{
    public class SqlServerConfiguration
    {
        public string ConnectionString { get; private set; }
        public string Schema { get; private set; }
        public string TableName { get; private set; }
    
        public SqlServerConfiguration(string connectionString):this(connectionString,"dbo","simplecqrs_event_store")
        {
        }

        public SqlServerConfiguration(string connectionString, string schema, string table)
        {
            ConnectionString = connectionString;
            Schema = schema;
            TableName = table;
        }
    }
}