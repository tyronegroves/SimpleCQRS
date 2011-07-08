using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using SimpleCqrs.Eventing;
using SimpleCqrs.EventStore.SqlServer.Serializers;

namespace SimpleCqrs.EventStore.SqlServer.Tests.Specs
{
    [Subject(typeof (JsonDomainEventSerializer))]
    public class when_serializing_a_SomethingHappenedEvent : with_automoqer
    {
        private Because of =
            () =>
                {
                    var serializer = new JsonDomainEventSerializer();
                    result = serializer.Serialize(new SomethingHappenedEvent
                                             {
                                                 AggregateRootId = new Guid("14802395-BAC1-4F4B-AF6D-855DADC9E824"),
                                                 EventDate = new DateTime(2015, 3, 31),
                                                 Sequence = 4,
                                                 ThisHappened = "Testing values."
                                             });
                };

        private It should_equal_a_json_representation_of_the_object =
            () => result.ShouldEqual(@"{""ThisHappened"":""Testing values."",""AggregateRootId"":""14802395bac14f4baf6d855dadc9e824"",""Sequence"":4,""EventDate"":""\/Date(1427778000000+0000)\/""}");

        private static string result;
    }
}
