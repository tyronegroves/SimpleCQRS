using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.EventStore.File
{
    public class FileEventStore : IEventStore
    {
        private readonly string baseDirectory;

        public FileEventStore(string baseDirectory)
        {
            this.baseDirectory = baseDirectory;
            if (!Directory.Exists(baseDirectory))
                Directory.CreateDirectory(baseDirectory);
        }

        public IEnumerable<DomainEvent> GetEvents(Guid aggregateRootId, int startSequence)
        {
            var aggregateRootDirectory = Path.Combine(baseDirectory, aggregateRootId.ToString());
            return null;
        }

        public void Insert(IEnumerable<DomainEvent> domainEvents)
        {
            foreach (var domainEvent in domainEvents)
            {
                var aggregateRootDirectory = Path.Combine(baseDirectory, domainEvent.AggregateRootId.ToString());
                if (!Directory.Exists(aggregateRootDirectory))
                    Directory.CreateDirectory(aggregateRootDirectory);

                var eventPath = Path.Combine(aggregateRootDirectory, string.Format("{0}.xml", domainEvent.Sequence));
                var serializer = new DataContractSerializer(domainEvent.GetType());
                var stream = new FileStream(eventPath, FileMode.Create);
                serializer.WriteObject(stream, domainEvent);
            }
        }
    }
}