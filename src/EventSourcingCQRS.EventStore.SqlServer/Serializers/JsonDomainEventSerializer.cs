using EventSourcingCQRS.Eventing;
using Newtonsoft.Json;


namespace EventSourcingCQRS.EventStore.SqlServer.Serializers
{
    public class JsonDomainEventSerializer : IDomainEventSerializer
    {
        public string Serialize(DomainEvent domainEvent)
        {

            var settings = new Newtonsoft.Json.JsonSerializerSettings
            {
                TypeNameAssemblyFormatHandling = Newtonsoft.Json.TypeNameAssemblyFormatHandling.Simple,
                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects,
            };

            var json = JsonConvert.SerializeObject(domainEvent, settings);
            return json;
        }

        public DomainEvent Deserialize(Type targetType, string serializedDomainEvent)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
                {
                    NamingStrategy = new Newtonsoft.Json.Serialization.CamelCaseNamingStrategy()
                },
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                TypeNameHandling = TypeNameHandling.Auto
            };

            var domainEvent = JsonConvert.DeserializeObject<DomainEvent>(serializedDomainEvent, settings);
            return domainEvent;
        }
    }

    public class BinaryDomainEventSerializer : IDomainEventSerializer
    {
        public string Serialize(DomainEvent domainEvent)
        {
            throw new NotImplementedException();

            //var options = new JsonSerializerOptions { WriteIndented = true };
            //using var stream = new MemoryStream();
            //JsonSerializer.Serialize(stream, domainEvent, options);
            //return Convert.ToBase64String(stream.ToArray());
        }

        public DomainEvent Deserialize(Type targetType, string serializedDomainEvent)
        {
            throw new NotImplementedException();
            //var formatter = new BinaryFormatter();
            //using var stream = new MemoryStream(Convert.FromBase64String(serializedDomainEvent));
            //var domainEvent = JsonSerializer.Deserialize<DomainEvent>(stream);
            //return domainEvent as DomainEvent;
        }
    }
}