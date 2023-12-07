using EventSourcingCQRS.Domain;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace EventSourcingCQRS.EventStore.MongoDb
{
    public class MongoEventStoreConfiguration : IMongoEventStoreConfiguration
    {
        private readonly ITypeCatalog _typeCatalog;

        public MongoEventStoreConfiguration(ITypeCatalog typeCatalog)
        {
            _typeCatalog = typeCatalog;
        }

        public void Configure()
        {
            var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElements", conventionPack, type => true);

            foreach (var t in _typeCatalog.LoadedTypes)
            {
                if (t.FullName != null && t.FullName.EndsWith("Event", StringComparison.InvariantCultureIgnoreCase))
                {
                    BsonClassMap.LookupClassMap(t);
                }
            }
        }
    }
}