using Azure;
using Newtonsoft.Json.Linq;

namespace EventSourcingCQRS.EventStore.CosmosDb
{
    public record CosmosDbEventData
    {
        /// <summary>
        /// this is the Aggregate id
        /// </summary>
        //public string PartitionKey { get; set; }

        public string Id { get; set; }

        /// <summary>
        /// aggregate version on the event
        /// </summary>
        //public string RowKey { get; set; }

        /// <summary>
        /// the event type
        /// </summary>
        public string EventType { get; init; }

        /// <summary>
        /// serialized event data
        /// </summary>
        //public byte[] EventData { get; init; }
        public JObject EventData { get; init; }

        /// <summary>
        /// aggregate version on the event
        /// </summary>
        //public long AggregateVersion { get; init; }

        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}

//using Azure;

//namespace EventSourcingCQRS.EventStore.CosmosDb
//{
//    public record CosmosDbEventData
//    {
//        /// <summary>
//        /// this is the Aggregate id
//        /// </summary>
//        public string PartitionKey { get; set; }

//        public string Id{ get; set; }

//        /// <summary>
//        /// aggregate version on the event
//        /// </summary>
//        public string RowKey { get; set; }

//        /// <summary>
//        /// the event type
//        /// </summary>
//        public string EventType { get; init; }

//        /// <summary>
//        /// serialized event data
//        /// </summary>
//        //public byte[] EventData { get; init; }
//        public string EventData { get; init; }

//        /// <summary>
//        /// aggregate version on the event
//        /// </summary>
//        public long AggregateVersion { get; init; }

//        public DateTimeOffset? Timestamp { get; set; }
//        public ETag ETag { get; set; }
//    }
//}