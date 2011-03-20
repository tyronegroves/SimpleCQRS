using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace SimpleCqrs.EventStore.SqlServer.Tests.Specs
{
    [Subject(typeof (SqlServerEventStore))]
    public class when_inserting_one_event : with_automoqer
    {
        private Because of =
            () =>
                {
                    var @event = new SomethingHappenedEvent
                                     {
                                         AggregateRootId = new Guid("8312E92C-DF1C-4970-A9D5-6414120C3CF7"),
                                         EventDate = DateTime.Parse("3/20/2010 3:01:04 AM"),
                                         Sequence = 2
                                     };
                    var store = Create<SqlServerEventStore>();
                    store.Insert(new []{@event});
                };

        private It should_execute_the_appropriate_sql_statement =
            () => GetMock<ISqlStatementRunner>()
                      .Verify(x => x.RunThisSql(@"Insert into Event_Store (EventType, AggregateRootId, EventDate, Sequence) 
Values ('SomethingHappened', '8312E92C-DF1C-4970-A9D5-6414120C3CF7', '3/20/2010 3:01:04 AM' ,'2');"), Times.Once());
    }
}
