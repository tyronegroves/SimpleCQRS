Feature: Insert events
	In order to add events to my SQL Server event store
	As a Simple CQRS developer
	I want to pass an array of events and have them added to the appropriate table

Background:
	Given the connection string to my database is
	"""
	Data Source=.\SQLEXPRESS;Initial Catalog=test;Integrated Security=True;MultipleActiveResultSets=True;
	"""

Scenario: Insert one domain event
	Given I have a SomethingHappenedEvent to be added to the store with the following values
	| Field           | Value                                |
	| EventDate       | 3/20/2010 3:01:04 AM                 |
	| AggregateRootId | 8312E92C-DF1C-4970-A9D5-6414120C3CF7 |
	| Sequence        | 2                                    |
	| ThisHappened    | something                            |
	And that event will serialize to 'Serialized Object'
	When I add the domain events to the store
	Then I should have the following events in the database
	| EventDate            | Data              | Sequence | AggregateRootId                      | EventType                                                                                           |
	| 3/20/2010 3:01:04 AM | Serialized Object | 2        | 8312E92C-DF1C-4970-A9D5-6414120C3CF7 | SimpleCqrs.EventStore.SqlServer.Tests.SomethingHappenedEvent, SimpleCqrs.EventStore.SqlServer.Tests |

Scenario: Insert two domain events
	Given I have a SomethingHappenedEvent to be added to the store with the following values
	| Field           | Value                                |
	| EventDate       | 3/20/2010 3:01:04 AM                 |
	| AggregateRootId | 8312E92C-DF1C-4970-A9D5-6414120C3CF7 |
	| Sequence        | 2                                    |
	| ThisHappened    | something                            |
	And that event will serialize to 'The First Record'
	And I have a SomethingElseHappenedEvent to be added to the store with the following values
	| Field           | Value                                |
	| EventDate       | 4/24/2010 3:01:04 AM                 |
	| AggregateRootId | C3579C12-C29B-4F65-8D83-B79AC5C85718 |
	| Sequence        | 4                                    |
	| SomeDataToStore | Testing                              |
	And that event will serialize to 'The Second Record'
	When I add the domain events to the store
	Then I should have the following events in the database
	| EventDate            | Data              | Sequence | AggregateRootId                      | EventType                                                                                               |
	| 3/20/2010 3:01:04 AM | The First Record  | 2        | 8312E92C-DF1C-4970-A9D5-6414120C3CF7 | SimpleCqrs.EventStore.SqlServer.Tests.SomethingHappenedEvent, SimpleCqrs.EventStore.SqlServer.Tests     |
	| 4/24/2010 3:01:04 AM | The Second Record | 4        | C3579C12-C29B-4F65-8D83-B79AC5C85718 | SimpleCqrs.EventStore.SqlServer.Tests.SomethingElseHappenedEvent, SimpleCqrs.EventStore.SqlServer.Tests |

Scenario: Inserting and retrieving an event
	Given I have a SomethingHappenedEvent to be added to the store with the following values
	| Field           | Value                                |
	| EventDate       | 3/20/2010 3:01:04 AM                 |
	| AggregateRootId | 8312E92C-DF1C-4970-A9D5-6414120C3CF7 |
	| Sequence        | 2                                    |
	| ThisHappened    | something                            |
	And I am choosing to use the Json Serializer
	When I add the domain events to the store
	Then I should get back the following SomethingHappenedEvents
	| EventDate            | AggregateRootId                      | Sequence | ThisHappened |
	| 3/20/2010 3:01:04 AM | 8312E92C-DF1C-4970-A9D5-6414120C3CF7 | 2        | something    |
