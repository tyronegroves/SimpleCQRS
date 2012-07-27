using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.EventStore.AzureBlob
{
    public class AzureBlobEventStore : IEventStore
    {
        private const string _aggregateRootIdMetadata = "AggregateRootId";
        private const string _domainEventSequenceMetadata = "DomainEventSequence";
        private const string _filenameMetadata = "Filename";
        private const string _formatFilename = "{0}/{1}.xml";
        private const string _contentType = "text/xml";

        private readonly CloudBlobContainer Container;
        private readonly DataContractSerializer Serializer;
        private readonly CloudBlobClient BlobClient;

        public AzureBlobEventStore(string containerName, ITypeCatalog typeCatalog, string connectionString)
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(connectionString);
            BlobClient = account.CreateCloudBlobClient();
            Container = BlobClient.GetContainerReference(containerName.ToLower());
            Container.CreateIfNotExist();

            var domainEventDerivedTypes = typeCatalog.GetDerivedTypes(typeof(DomainEvent));
            Serializer = new DataContractSerializer(typeof(DomainEvent), domainEventDerivedTypes);
        }

        public IEnumerable<DomainEvent> GetEvents(Guid aggregateRootId, int startSequence)
        {
            BlockingCollection<DomainEvent> result = new BlockingCollection<DomainEvent>();

            IEnumerable<CloudBlob> cloudBlobs = Container.GetDirectoryReference(aggregateRootId.ToString().ToLower())
                .ListBlobs()
                .Select(eventInfo => Container.GetBlobReference(eventInfo.Uri.ToString()));
            Parallel.ForEach(cloudBlobs, blob =>
                {
                    if (Valid(aggregateRootId, startSequence, blob))
                        using (BlobStream stream = blob.OpenRead())
                        {
                            DomainEvent domainEvent = (DomainEvent)Serializer.ReadObject(stream);
                            lock (result)
                                result.Add(domainEvent);
                            stream.Close();
                        }
                });
            return result.OrderBy(b => b.Sequence);
        }

        private static bool Valid(Guid aggregateRootId, int startSequence, CloudBlob blob)
        {
            blob.FetchAttributes();
            return blob.Metadata[_aggregateRootIdMetadata] == aggregateRootId.ToString().ToLower()
                   && int.Parse(blob.Metadata[_domainEventSequenceMetadata]) >= startSequence;
        }

        public void Insert(IEnumerable<DomainEvent> domainEvents)
        {
            Parallel.ForEach(domainEvents, Insert);
        }

        private void Insert(DomainEvent domainEvent)
        {
            CloudBlob blob =
                Container.GetBlobReference(string.Format(_formatFilename, domainEvent.AggregateRootId.ToString().ToLower(),
                                                         domainEvent.Sequence));
            blob.Properties.ContentType = _contentType;
            using (var stream = blob.OpenWrite())
            {
                Serializer.WriteObject(stream, domainEvent);
                stream.Close();
            }
            blob.Metadata.Add(_aggregateRootIdMetadata, domainEvent.AggregateRootId.ToString());
            blob.Metadata.Add(_domainEventSequenceMetadata, domainEvent.Sequence.ToString(CultureInfo.InvariantCulture));
            blob.Metadata.Add(_filenameMetadata,
                              string.Format(_formatFilename, domainEvent.AggregateRootId.ToString().ToLower(),
                                            domainEvent.Sequence));
            blob.SetMetadata();
        }

        public IEnumerable<DomainEvent> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DomainEvent> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes, Guid aggregateRootId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DomainEvent> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }
    }
}