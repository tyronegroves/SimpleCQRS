﻿using SimpleCqrs.Eventing;

namespace SimpleCqrs.EventStore.SqlServer.Tests
{
    public class SomethingHappenedEvent : DomainEvent
    {
        public string ThisHappened { get; set; }
    }
}