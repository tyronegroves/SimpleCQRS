using System.Text.Json;

namespace EventSourcingCQRS.EventStore.CosmosDb
{
    public class CosmosDbConnectionStringBuilder

    {
        private string _accountEndpoint;
        private string _accountKey;
        private string _connectionMode;
        private string _consistencyLevel;
        private string _databaseName;
        private int _gatewayModeMaxConnectionLimit;
        private int _gatewayModeMaxRetryAttempts;
        private int _gatewayModeMaxRetryWaitTimeInSeconds;
        private int _maxConnectionLimit;
        private int _retryAttemptsOnThrottledRequests;
        private int _retryWaitTimeInSeconds;
        private JsonSerializerOptions _serializerOptions;
        private bool _useMultipleWriteLocations;

        public CosmosDbConnectionStringBuilder()
        {
            // Set default values for properties
            this._connectionMode = "Direct";
            this._consistencyLevel = "Session";
            this._gatewayModeMaxConnectionLimit = 50;
            this._gatewayModeMaxRetryAttempts = 0;
            this._gatewayModeMaxRetryWaitTimeInSeconds = 30;
            this._maxConnectionLimit = 50;
            this._retryAttemptsOnThrottledRequests = 9;
            this._retryWaitTimeInSeconds = 1;
            this._serializerOptions = new JsonSerializerOptions();
            this._useMultipleWriteLocations = false;
        }



        public string AccountEndpoint
        {
            get => this._accountEndpoint;
            set => this._accountEndpoint = value;
        }

        public string AccountKey
        {
            get => this._accountKey;
            set => this._accountKey = value;
        }

        public string ConnectionMode
        {
            get => this._connectionMode;
            set => this._connectionMode = value;
        }

        public string ConsistencyLevel
        {
            get => this._consistencyLevel;
            set => this._consistencyLevel = value;
        }

        public string DatabaseName
        {
            get => this._databaseName;
            set => this._databaseName = value;
        }

        public int GatewayModeMaxConnectionLimit
        {
            get => this._gatewayModeMaxConnectionLimit;
            set => this._gatewayModeMaxConnectionLimit = value;
        }

        public int GatewayModeMaxRetryAttempts
        {
            get => this._gatewayModeMaxRetryAttempts;
            set => this._gatewayModeMaxRetryAttempts = value;
        }

        public int GatewayModeMaxRetryWaitTimeInSeconds
        {
            get => this._gatewayModeMaxRetryWaitTimeInSeconds;
            set => this._gatewayModeMaxRetryWaitTimeInSeconds = value;
        }

        public int MaxConnectionLimit
        {
            get => this._maxConnectionLimit;
            set => this._maxConnectionLimit = value;
        }

        public int RetryAttemptsOnThrottledRequests
        {
            get => this._retryAttemptsOnThrottledRequests;
            set => this._retryAttemptsOnThrottledRequests = value;
        }

        public int RetryWaitTimeInSeconds
        {
            get => this._retryWaitTimeInSeconds;
            set => this._retryWaitTimeInSeconds = value;
        }

        public JsonSerializerOptions SerializerOptions
        {
            get => this._serializerOptions;
            set => this._serializerOptions = value;
        }

        public bool UseMultipleWriteLocations
        {
            get => this._useMultipleWriteLocations;
            set => this._useMultipleWriteLocations = value;
        }

        public static CosmosDbConnectionStringBuilder ParseConnectionString(string connectionString)
        {
            CosmosDbConnectionStringBuilder builder = new CosmosDbConnectionStringBuilder();

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));
            }

            string[] parts = connectionString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string part in parts)
            {
                //string[] keyValue = part.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

                var keyValue = new string[2];
                var index = part.IndexOf("="); // Find the index of the first occurrence of the separator
                if (index == -1)
                {
                    throw new ArgumentException($"Invalid key-value pair: {part}", nameof(connectionString));
                }
                else
                {
                    
                    keyValue[0] = part.Substring(0, index); // Extract the substring before the separator
                    keyValue[1] = part.Substring(index + 1); // Extract the substring after the separator
                }


                var key = keyValue[0].Trim().ToLowerInvariant();
                var value = keyValue[1].Trim();

                switch (key)
                {
                    case "accountendpoint":
                        builder.AccountEndpoint = value;
                        break;

                    case "accountkey":
                        builder.AccountKey = value;
                        break;

                    case "connectionmode":
                        builder.ConnectionMode = value;
                        break;

                    case "consistencylevel":
                        builder.ConsistencyLevel = value;
                        break;

                    case "database":
                    case "databasename":
                        builder.DatabaseName = value;
                        break;

                    case "gatewaymodemaxconnectionlimit":
                        if (!int.TryParse(value, out int gatewayModeMaxConnectionLimit))
                        {
                            throw new ArgumentException($"Invalid value for {key}: {value}", nameof(connectionString));
                        }

                        builder.GatewayModeMaxConnectionLimit = gatewayModeMaxConnectionLimit;
                        break;

                    case "gatewaymodemaxretryattempts":
                        if (!int.TryParse(value, out int gatewayModeMaxRetryAttempts))
                        {
                            throw new ArgumentException($"Invalid value for {key}: {value}", nameof(connectionString));
                        }

                        builder.GatewayModeMaxRetryAttempts = gatewayModeMaxRetryAttempts;
                        break;

                    case "gatewaymodemaxretrywaittimeinseconds":
                        if (!int.TryParse(value, out int gatewayModeMaxRetryWaitTimeInSeconds))
                        {
                            throw new ArgumentException($"Invalid value for {key}: {value}", nameof(connectionString));
                        }

                        builder.GatewayModeMaxRetryWaitTimeInSeconds = gatewayModeMaxRetryWaitTimeInSeconds;
                        break;

                    case "maxconnectionlimit":
                        if (!int.TryParse(value, out int maxConnectionLimit))
                        {
                            throw new ArgumentException($"Invalid value for {key}: {value}", nameof(connectionString));
                        }

                        builder.MaxConnectionLimit = maxConnectionLimit;
                        break;

                    case "retryattemptsonthrottledrequests":
                        if (!int.TryParse(value, out int retryAttemptsOnThrottledRequests))
                        {
                            throw new ArgumentException($"Invalid value for {key}: {value}", nameof(connectionString));
                        }

                        builder.RetryAttemptsOnThrottledRequests = retryAttemptsOnThrottledRequests;
                        break;

                    case "retrywaittimeinseconds":
                        if (!int.TryParse(value, out int retryWaitTimeInSeconds))
                        {
                            throw new ArgumentException($"Invalid value for {key}: {value}", nameof(connectionString));
                        }

                        builder.RetryWaitTimeInSeconds = retryWaitTimeInSeconds;
                        break;

                    //case "serializeroptions":
                    //    try
                    //    {
                    //        builder.SerializerOptions = JsonSerializerOptions.Parse(value);
                    //    }
                    //    catch (JsonException ex)
                    //    {
                    //        throw new ArgumentException($"Invalid value for {key}: {value}. {ex.Message}",
                    //            nameof(connectionString));
                    //    }

                    //    break;

                    case "usemultiplewritelocations":
                        if (!bool.TryParse(value, out bool useMultipleWriteLocations))
                        {
                            throw new ArgumentException($"Invalid value for {key}: {value}", nameof(connectionString));
                        }

                        builder.UseMultipleWriteLocations = useMultipleWriteLocations;
                        break;
                    default:
                    {
                        throw new Exception("value not recognised");
                    }
                }
            }
            return builder;
        }
    }
}

