using EventSourcingCQRS.Domain;
using EventSourcingCQRS.Eventing;
using EventSourcingCQRS.EventStore.MongoDb.RetryPolicies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Polly.Registry;
using Polly.Wrap;

namespace EventSourcingCQRS.EventStore.MongoDb.EventStore
{
    public class MongoSnapshotStore : ISnapshotStore
    {
        private const string SnapshotsCollection = "Snapshots";

        private readonly string _className;
        private readonly ILogger<MongoSnapshotStore> _logger;
        private readonly IMongoCollection<Snapshot> _mongoCollection;
        private readonly IMongoDatabase _mongoDatabase;
        private readonly AsyncPolicyWrap _asyncRetryPolicy;

        public MongoSnapshotStore(ILogger<MongoSnapshotStore> logger, IMongoClient mongoClient,
            IMongoSnapshotStoreConfiguration mongoEventStoreConfiguration, IConfiguration configuration,
            IReadOnlyPolicyRegistry<string> policyRegistry)
        {
            _className = GetType().FullName;
            _logger = logger;
            var databaseName = configuration.GetSection("Databases:EventStore").Value;
            mongoEventStoreConfiguration.Configure();
            _mongoDatabase = mongoClient.GetDatabase(databaseName);
            _mongoCollection = _mongoDatabase
                .GetCollection<Snapshot>(SnapshotsCollection);
            _asyncRetryPolicy = policyRegistry.Get<AsyncPolicyWrap>(EventStoreRetryPolicy.RetryPolicyName);
        }

        public async Task<Snapshot> GetSnapshot(Guid aggregateRootId, CancellationToken cancellationToken)
        {
            Snapshot documents = null;
            await _asyncRetryPolicy.ExecuteAsync(async () =>
            {
                documents = await _mongoCollection
                    .Find(x => x.AggregateRootId == aggregateRootId)
                    .SingleOrDefaultAsync(cancellationToken);
            });
            return documents;
        }

        public async Task SaveSnapshot<TSnapshot>(TSnapshot snapshot, CancellationToken cancellationToken) where TSnapshot : Snapshot
        {
            var snapshotsCollection = _mongoDatabase.GetCollection<TSnapshot>("Snapshots");
            var replaceOptions = new ReplaceOptions
            {
                IsUpsert = true
            };
            var filter = Builders<TSnapshot>.Filter.Eq("AggregateRootId", snapshot.AggregateRootId);
            await snapshotsCollection.ReplaceOneAsync(filter, snapshot, replaceOptions,cancellationToken);
        }


    }
}