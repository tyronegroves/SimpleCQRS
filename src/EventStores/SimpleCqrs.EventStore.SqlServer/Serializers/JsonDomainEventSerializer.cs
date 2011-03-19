using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using ServiceStack.Text;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.EventStore.SqlServer.Serializers
{
    public class JsonDomainEventSerializer : IDomainEventSerializer
    {

        public string Serialize(DomainEvent domainEvent)
        {
            return JsonSerializer.SerializeToString(domainEvent, typeof(DomainEvent));
        }

        public DomainEvent Deserialize(Type targetType, string serializedDomainEvent)
        {
            return (DomainEvent)JsonSerializer.DeserializeFromString(serializedDomainEvent, targetType);
        }
    }

    public class BinaryDomainEventSerializer : IDomainEventSerializer
    {
        public string Serialize(DomainEvent domainEvent) {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream()) {
                formatter.Serialize(stream, domainEvent);
                stream.Flush();
                stream.Position = 0;
                return Convert.ToBase64String(stream.ToArray());
            }
        }

        public DomainEvent Deserialize(Type targetType, string serializedDomainEvent) {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream(Convert.FromBase64String(serializedDomainEvent))) {
                return (DomainEvent)formatter.Deserialize(stream);
            }
        }
    }

}