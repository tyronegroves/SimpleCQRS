Feature: Get Events
	In order to retrieve events from my SQL Server event store
	As a Simple CQRS developer
	I want to pass a request to retrieve events and get the appropriate events back

Background:
	Given the connection string to my database is
	"""
	Data Source=DEGWCAUTHOND2\SQLEXPRESS;Initial Catalog=test;Integrated Security=True;MultipleActiveResultSets=True;
	"""

Scenario: Get Events by aggregate root id
	Given I have the following events in the database
	| EventDate            | Data               | Sequence | AggregateRootId                      | EventType                                                                                                                                                  |
	| 3/20/2010 3:01:04 AM | Serialized Object  | 1        | 8312E92C-DF1C-4970-A9D5-6414120C3CF7 | SimpleCqrs.EventStore.SqlServer.Tests.SomethingHappenedEvent, SimpleCqrs.EventStore.SqlServer.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null |
    | 3/20/2010 4:01:04 AM | Serialized Objecta | 2        | D50E4D4F-0893-45B2-92F8-897514812A91 | SimpleCqrs.EventStore.SqlServer.Tests.SomethingHappenedEvent, SimpleCqrs.EventStore.SqlServer.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null |
	| 3/20/2010 5:01:04 AM | Serialized Object2 | 3        | 8312E92C-DF1C-4970-A9D5-6414120C3CF7 | SimpleCqrs.EventStore.SqlServer.Tests.SomethingElseHappenedEvent, SimpleCqrs.EventStore.SqlServer.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null |
	And deserializing 'Serialized Object' will return a SomethingHappenedEvent with the following data
	| Field           | Value |
	| Sequence        | 97    |
	And deserializing 'Serialized Object2' will return a SomethingElseHappenedEvent with the following data
	| Field           | Value |
	| Sequence        | 98    |
	When I retrieve the domain events for '8312E92C-DF1C-4970-A9D5-6414120C3CF7'
	Then I should get back the following DomainEvents
	| Sequence |
	| 97       |
	| 98       | 

Scenario: Get Events by sequence
	Given I have the following events in the database
	| EventDate            | Data               | Sequence | AggregateRootId                      | EventType                                                                                                                                                  |
	| 3/20/2010 3:01:04 AM | Serialized Object  | 1        | 8312E92C-DF1C-4970-A9D5-6414120C3CF7 | SimpleCqrs.EventStore.SqlServer.Tests.SomethingHappenedEvent, SimpleCqrs.EventStore.SqlServer.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null |
    | 3/20/2010 4:01:04 AM | Serialized Objecta | 2        | D50E4D4F-0893-45B2-92F8-897514812A91 | SimpleCqrs.EventStore.SqlServer.Tests.SomethingHappenedEvent, SimpleCqrs.EventStore.SqlServer.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null |
	| 3/20/2010 5:01:04 AM | Serialized Object2 | 3        | 8312E92C-DF1C-4970-A9D5-6414120C3CF7 | SimpleCqrs.EventStore.SqlServer.Tests.SomethingElseHappenedEvent, SimpleCqrs.EventStore.SqlServer.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null |
	And deserializing 'Serialized Object' will return a SomethingHappenedEvent with the following data
	| Field           | Value |
	| Sequence        | 97    |
	And deserializing 'Serialized Object2' will return a SomethingElseHappenedEvent with the following data
	| Field           | Value |
	| Sequence        | 98    |
	When I retrieve the domain events for '8312E92C-DF1C-4970-A9D5-6414120C3CF7' and sequence 2
	Then I should get back the following DomainEvents
	| Sequence |
	| 98       | 

Scenario: Get events with one event type
	Given I have the following events in the database
	| EventDate            | Data               | Sequence | AggregateRootId                      | EventType                                                                                                                                                  |
	| 3/20/2010 3:01:04 AM | Serialized Object  | 1        | 8312E92C-DF1C-4970-A9D5-6414120C3CF7 | SimpleCqrs.EventStore.SqlServer.Tests.SomethingHappenedEvent, SimpleCqrs.EventStore.SqlServer.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null |
    | 3/20/2010 4:01:04 AM | Serialized Object2 | 2        | D50E4D4F-0893-45B2-92F8-897514812A91 | SimpleCqrs.EventStore.SqlServer.Tests.SomethingHappenedEvent, SimpleCqrs.EventStore.SqlServer.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null |
	| 3/20/2010 5:01:04 AM | Serialized Object3 | 3        | 8312E92C-DF1C-4970-A9D5-6414120C3CF7 | SimpleCqrs.EventStore.SqlServer.Tests.SomethingElseHappenedEvent, SimpleCqrs.EventStore.SqlServer.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null |
	And deserializing 'Serialized Object' will return a SomethingHappenedEvent with the following data
	| Field           | Value |
	| Sequence        | 97    |
	And deserializing 'Serialized Object2' will return a SomethingHappenedEvent with the following data
	| Field           | Value |
	| Sequence        | 98    |
	And deserializing 'Serialized Object3' will return a SomethingElseHappenedEvent with the following data
	| Field           | Value |
	| Sequence        | 99    |
	When I retrieve the domain events for the following types
	| Type                                                                                                                                                           |
	| SimpleCqrs.EventStore.SqlServer.Tests.SomethingHappenedEvent, SimpleCqrs.EventStore.SqlServer.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null     |
	Then I should get back the following DomainEvents
	| Sequence |
	| 97       | 
	| 98       |

Scenario: Get events with two event types
	Given I have the following events in the database
	| EventDate            | Data               | Sequence | AggregateRootId                      | EventType                                                                                                                                                  |
	| 3/20/2010 3:01:04 AM | Serialized Object  | 1        | 8312E92C-DF1C-4970-A9D5-6414120C3CF7 | SimpleCqrs.EventStore.SqlServer.Tests.SomethingHappenedEvent, SimpleCqrs.EventStore.SqlServer.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null |
    | 3/20/2010 4:01:04 AM | Serialized Object2 | 2        | D50E4D4F-0893-45B2-92F8-897514812A91 | SimpleCqrs.EventStore.SqlServer.Tests.SomethingHappenedEvent, SimpleCqrs.EventStore.SqlServer.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null |
	| 3/20/2010 5:01:04 AM | Serialized Object3 | 3        | 8312E92C-DF1C-4970-A9D5-6414120C3CF7 | SimpleCqrs.EventStore.SqlServer.Tests.SomethingElseHappenedEvent, SimpleCqrs.EventStore.SqlServer.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null |
	And deserializing 'Serialized Object' will return a SomethingHappenedEvent with the following data
	| Field           | Value |
	| Sequence        | 97    |
	And deserializing 'Serialized Object2' will return a SomethingHappenedEvent with the following data
	| Field           | Value |
	| Sequence        | 98    |
	And deserializing 'Serialized Object3' will return a SomethingElseHappenedEvent with the following data
	| Field           | Value |
	| Sequence        | 99    |
	When I retrieve the domain events for the following types
	| Type                                                                                                                                                           |
	| SimpleCqrs.EventStore.SqlServer.Tests.SomethingHappenedEvent, SimpleCqrs.EventStore.SqlServer.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null     |
	| SimpleCqrs.EventStore.SqlServer.Tests.SomethingElseHappenedEvent, SimpleCqrs.EventStore.SqlServer.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null |
	Then I should get back the following DomainEvents
	| Sequence |
	| 97       | 
	| 98       |
	| 99       |