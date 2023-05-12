//using ServiceStack.Text;
//using SimpleCqrs.Eventing;

using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
//using System.Text.Json;
using EventSourcingCQRS.Eventing;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

//using JsonSerializer = System.Text.Json.JsonSerializer;

namespace EventSourcingCQRS.EventStore.SqlServer.Serializers
{
    public class JsonDomainEventSerializer : IDomainEventSerializer
    {
        public string Serialize(DomainEvent domainEvent)
        {
            ////            return $"Cannot find type '{domainEvent.GetType()}', yet the type is in the event store. Are you sure you haven't changed a class name or something arising from mental dullness?";
            //var options = new JsonSerializerOptions { WriteIndented = true };
            ////using var stream = new MemoryStream();
            ////JsonSerializer.Serialize(stream, domainEvent, options);
            ////var reader = new StreamReader(stream);
            ////var text = reader.ReadToEnd();
            ////return text;

            //return JsonSerializer.Serialize(domainEvent,domainEvent.GetType(), options);



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
            //var options = new JsonSerializerOptions
            //{
            //    IncludeFields=true, 
            //    PropertyNameCaseInsensitive = true,

            //};
            //var domainEvent = JsonSerializer.Deserialize(serializedDomainEvent, targetType, options);
            //return domainEvent as DomainEvent;
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
                {
                    NamingStrategy = new Newtonsoft.Json.Serialization.CamelCaseNamingStrategy()
                },
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                TypeNameHandling = TypeNameHandling.Auto
            };

            DomainEvent domainEvent = JsonConvert.DeserializeObject<DomainEvent>(serializedDomainEvent, settings);
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