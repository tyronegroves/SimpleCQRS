using Azure;
using Azure.Data.Tables;

namespace EventSourcingCQRS.EventStore.AzureTableStorage
{
    public record EventData : ITableEntity
    {
        /// <summary>
        /// this is the Aggregate id
        /// </summary>
        public string PartitionKey { get; set; }

        /// <summary>
        /// aggregate version on the event
        /// </summary>
        public string RowKey { get; set; }

        /// <summary>
        /// the event type
        /// </summary>
        public string EventType { get; init; }

        /// <summary>
        /// serialized event data
        /// </summary>
        //public byte[] Data { get; init; }
        public string Data { get; init; }

        /// <summary>
        /// aggregate version on the event
        /// </summary>
        public long AggregateVersion { get; init; }

        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}