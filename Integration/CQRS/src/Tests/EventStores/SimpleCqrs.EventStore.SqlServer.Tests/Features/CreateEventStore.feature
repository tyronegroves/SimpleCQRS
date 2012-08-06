Feature: Create event store
	In order to insert and retrieve events from the event store
	As a Simple CQRS developer
	I want the event store to be created if it does not exist

Background:
	Given the connection string to my database is
	"""
	Data Source=.\SQLEXPRESS;Initial Catalog=test;Integrated Security=True;MultipleActiveResultSets=True;
	"""

Scenario: The event store does not exist before inserting
	Given the EventStore table does not exist
	And I have a SomethingHappenedEvent to be added to the store with the following values
	| Field           | Value                                |
	| EventDate       | 3/20/2010 3:01:04 AM                 |
	| AggregateRootId | 8312E92C-DF1C-4970-A9D5-6414120C3CF7 |
	| Sequence        | 2                                    |
	| ThisHappened    | something                            |
	And that event will serialize to 'Serialized Object'
	When I add the domain events to the store
	Then I should have the following events in the database
	| EventDate            |
	| 3/20/2010 3:01:04 AM |

Scenario: The event store does not exist before retrieving events by aggregate root
	Given the EventStore table does not exist
	When I retrieve the domain events for '8312E92C-DF1C-4970-A9D5-6414120C3CF7'
	Then I should get back the following DomainEvents
	| Sequence |

Scenario: The event store does not exist before retrieving events by event type
	Given the EventStore table does not exist
	When I retrieve the domain events for the following types
	| Type                                                                                                                                                           |
	| SimpleCqrs.EventStore.SqlServer.Tests.SomethingHappenedEvent, SimpleCqrs.EventStore.SqlServer.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null     |
	Then I should get back the following DomainEvents
	| Sequence |