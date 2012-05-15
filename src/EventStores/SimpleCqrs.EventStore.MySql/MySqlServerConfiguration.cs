namespace SimpleCqrs.EventStore.MySql
{
    public class MySqlServerConfiguration
    {
        private readonly string connectionString;

        public MySqlServerConfiguration(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public string ConnectionString
        {
            get { return connectionString; }
        }
    }
}