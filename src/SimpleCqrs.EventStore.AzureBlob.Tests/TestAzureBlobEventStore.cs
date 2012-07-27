using System;
using System.Collections.Generic;
using System.Linq;
using AutoMoq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using SimpleCqrs.Core.Tests.Commanding;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.EventStore.AzureBlob.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private const string _containerName = "eventstorecontainer";
        private const string _azureBlobConnectionString = "UseDevelopmentStorage=true";

        private AutoMoqer mocker;
        private MockServiceLocator serviceLocator;

        protected CloudStorageAccount Account { get; set; }
        protected CloudBlobClient BlobClient { get; set; }
        protected CloudBlobContainer Container { get; set; }

        [TestInitialize]
        public void SetupMocksForAllTests()
        {
            mocker = new AutoMoqer();
            serviceLocator = new MockServiceLocator(mocker);

            Account = CloudStorageAccount.Parse(_azureBlobConnectionString);
            BlobClient = Account.CreateCloudBlobClient();
            Container = BlobClient.GetContainerReference(_containerName.ToLower());
            // this is to guarantee that we are dealing with a clean empty container
            Container.CreateIfNotExist();
            Container.Delete();
            Container.CreateIfNotExist();
        }

        [TestCleanup]
        public void CleanUpAzureBlob()
        {
            //Container.Delete();
        }

        [TestMethod]
        public void TestMethod_InsertAndRetrieveEvents()
        {
            var eventStore = new AzureBlobEventStore(_containerName, serviceLocator.Resolve<ITypeCatalog>(), _azureBlobConnectionString);
            var id = Guid.NewGuid();
            var dateTime = DateTime.UtcNow;
            eventStore.Insert(GetMockDomainEvents(id, dateTime, 3));
            List<DomainEvent> testResult = new List<DomainEvent>(eventStore.GetEvents(id, 2));
            Assert.AreEqual(2, testResult.Count);

            List<DomainEvent> result = new List<DomainEvent>(eventStore.GetEvents(id, 0));
            Assert.AreEqual(3, result.Count);

            foreach (DomainEvent domainEvent in result)
            {
                Assert.AreEqual(id, domainEvent.AggregateRootId);
                Assert.AreEqual(dateTime, domainEvent.EventDate);
            }
            Assert.AreEqual(1, result[0].Sequence);
            Assert.AreEqual(2, result[1].Sequence);
            Assert.AreEqual(3, result[2].Sequence);
        }

        private static DomainEvent GetMockDomainEvent(Guid id, DateTime dateTime, ref int i)
        {
            return new DomainEvent { AggregateRootId = id, EventDate = dateTime, Sequence = i++ };
        }

        private static IEnumerable<DomainEvent> GetMockDomainEvents(Guid id, DateTime dateTime, int count)
        {
            int i = 1;
            for (int j = 0; j < count; j++)
            {
                yield return GetMockDomainEvent(id, dateTime, ref i);
            }
        }
    }
}
