using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleCqrs.Commanding;
using SimpleCqrs.Domain;
using SimpleCqrs.Eventing;
using SimpleCqrs.EventStore.SqlServer;
using SimpleCqrs.EventStore.SqlServer.Serializers;

namespace SqlServerEventStoreSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = new Program();
            
           p.DoBinarySerializedEvents();

            p.DoJsonSerializedEvents();
        }

        void DoBinarySerializedEvents() {
            var json = new BinarySampleRuntime();
            json.Start();
            
            var id = Guid.NewGuid();

            var serializer = new BinaryDomainEventSerializer();

            var obj = serializer.Serialize(new FooCreatedEvent());

            

            var root = new FooRoot();
            root.CreateMe(id);

            var repo = json.ServiceLocator.Resolve<IDomainRepository>();

            repo.Save(root);

            var newRoot = repo.GetById<FooRoot>(id);

            Console.WriteLine(String.Format("Id : {0}, Type : {1}", newRoot.Id, newRoot.GetType()));

            json.Shutdown();
        }

        void DoJsonSerializedEvents()
        {
            var json = new JsonSampleRuntime();
            json.Start();
         
            var id = Guid.NewGuid();

            var root = new FooRoot();
            root.CreateMe(id);
            var repo = json.ServiceLocator.Resolve<IDomainRepository>();
            
            repo.Save(root);

            var newRoot = repo.GetById<FooRoot>(id);

            Console.WriteLine(String.Format("Id : {0}, Type : {1}", newRoot.Id, newRoot.GetType()));
            json.Shutdown();
        }
    }

    public class BinarySampleRuntime : SimpleCqrs.SimpleCqrsRuntime<SimpleCqrs.Unity.UnityServiceLocator>
    {
        protected override IEventStore GetEventStore(SimpleCqrs.IServiceLocator serviceLocator) {
            var configuration = new SqlServerConfiguration("Server=(local)\\sqlexpress;Database=test_event_store;Trusted_Connection=True;",
                "dbo", "binary_event_store");
            return new SqlServerEventStore(configuration, new BinaryDomainEventSerializer());
        }
    }

    public class JsonSampleRuntime : SimpleCqrs.SimpleCqrsRuntime<SimpleCqrs.Unity.UnityServiceLocator>
    {
        protected override IEventStore GetEventStore(SimpleCqrs.IServiceLocator serviceLocator)
        {
            var configuration = new SqlServerConfiguration("Server=(local)\\sqlexpress;Database=test_event_store;Trusted_Connection=True;",
                "dbo", "json_event_store");
            return new SqlServerEventStore(configuration, new JsonDomainEventSerializer());
        }
    }

    [Serializable]
    public class FooCreatedEvent : DomainEvent
    {
        public string Bar { get; set; }
    }

    public class FooRoot : AggregateRoot
    {
        public void CreateMe(Guid id)
        {
            Apply(new FooCreatedEvent{AggregateRootId = id, Bar = "foobar"});
        }

        public void OnFooCreated(FooCreatedEvent domainEvent)
        {
            Id = domainEvent.AggregateRootId;
        }
    }
}
