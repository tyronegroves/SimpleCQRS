using EventSourcingCQRS.Domain;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace EventSourcingCQRS.EventStore.MongoDb
{
    public class MongoSnapshotStoreConfiguration : IMongoSnapshotStoreConfiguration
    {
        private readonly ITypeCatalog _typeCatalog;

        public MongoSnapshotStoreConfiguration(ITypeCatalog typeCatalog)
        {
            _typeCatalog = typeCatalog;
        }

        public void Configure()
        {
            var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElements", conventionPack, type => true);

            var snapShotters = _typeCatalog.LoadedTypes.Where(t =>
                t.FullName != null && t.FullName.EndsWith("Snapshot", StringComparison.InvariantCultureIgnoreCase));

            foreach (var t in snapShotters)
            {
                BsonClassMap.LookupClassMap(t);
            }

            //foreach (var t in _typeCatalog.LoadedTypes)
            //{
            //    if (t.FullName != null && t.FullName.EndsWith("Snapshot", StringComparison.InvariantCultureIgnoreCase))
            //    {
            //        BsonClassMap.LookupClassMap(t);
            //    }
            //}
        }
    }
}