using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.EventStore.File
{
    public class FileEventStore : IEventStore
    {
        private readonly string baseDirectory;
        private readonly DataContractSerializer serializer;

        public FileEventStore(string baseDirectory, ITypeCatalog typeCatalog)
        {
            this.baseDirectory = baseDirectory;
            if (!Directory.Exists(baseDirectory))
                Directory.CreateDirectory(baseDirectory);

            var domainEventDerivedTypes = typeCatalog.GetDerivedTypes(typeof(DomainEvent));
            serializer = new DataContractSerializer(typeof(DomainEvent), domainEventDerivedTypes);
        }

        public IEnumerable<DomainEvent> GetEvents(Guid aggregateRootId, int startSequence)
        {
            var eventInfos = GetEventInfosForAggregateRoot(aggregateRootId, startSequence);
            var domainEvents = new List<DomainEvent>();
            foreach (var eventInfo in eventInfos)
            {
                using (var stream = System.IO.File.OpenRead(eventInfo.FilePath))
                {
                    var domainEvent = (DomainEvent)serializer.ReadObject(stream);
                    domainEvents.Add(domainEvent);
                }
            }
        
            return domainEvents;
        }

        public void Insert(IEnumerable<DomainEvent> domainEvents)
        {
            foreach (var domainEvent in domainEvents)
            {
                var aggregateRootDirectory = Path.Combine(baseDirectory, domainEvent.AggregateRootId.ToString());
                if (!Directory.Exists(aggregateRootDirectory))
                    Directory.CreateDirectory(aggregateRootDirectory);

                var eventPath = Path.Combine(aggregateRootDirectory, string.Format("{0}.xml", domainEvent.Sequence));
                using(var stream = new FileStream(eventPath, FileMode.Create))
                {
                    serializer.WriteObject(stream, domainEvent);
                }
            }
        }

        private IEnumerable<dynamic> GetEventInfosForAggregateRoot(Guid aggregateRootId, int startSequence)
        {
            var aggregateRootDirectory = Path.Combine(baseDirectory, aggregateRootId.ToString());
            return from filePath in Directory.GetFiles(aggregateRootDirectory)
                   let fileName = Path.GetFileNameWithoutExtension(filePath)
                   where fileName != null
                   let sequence = int.Parse(fileName)
                   where sequence > startSequence
                   select new { Sequence = sequence, FilePath = filePath };
        }
    }
}