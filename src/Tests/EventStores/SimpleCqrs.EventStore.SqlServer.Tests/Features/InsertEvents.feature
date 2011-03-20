Feature: Insert events
	In order to add events to my SQL Server event store
	As a Simple CQRS developer
	I want to pass an array of events and have them added to the appropriate table

# Special Note:  I think it's probably better to test the final results 
# in the SQL server, for now I'm just going to test the SQL statements
# that are to be run against the DB.  I think it's worth demonstrating 
# that you can bend these types of specs to fit what you want (instead
# of just throwing away this type of testing alltogether).

Scenario: Insert one domain event
	Given I have a SomethingHappenedEvent to be added to the store with the following values
	| Field           | Value                                |
	| EventDate       | 3/20/2010 3:01:04 AM                 |
	| AggregateRootId | 8312E92C-DF1C-4970-A9D5-6414120C3CF7 |
	| Sequence        | 2                                    |
	And that event will serialize to 'Serialized Object'
	When I add the domain events to the store
	Then the following SQL statement should be run
	"""
	Insert into Event_Store (EventType, AggregateRootId, EventDate, Sequence, [Data]) 
	Values ('SimpleCqrs.EventStore.SqlServer.Tests.SomethingHappenedEvent, SimpleCqrs.EventStore.SqlServer.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null', '8312E92C-DF1C-4970-A9D5-6414120C3CF7', '3/20/2010 3:01:04 AM' ,'2', 'Serialized Object');
	"""
Scenario: Insert two domain events
	Given I have a SomethingHappenedEvent to be added to the store with the following values
	| Field           | Value                                |
	| EventDate       | 3/20/2010 3:01:04 AM                 |
	| AggregateRootId | 8312E92C-DF1C-4970-A9D5-6414120C3CF7 |
	| Sequence        | 2                                    |
	Given I have a SomethingElseHappenedEvent to be added to the store with the following values
	| Field           | Value                                |
	| EventDate       | 4/24/2010 3:01:04 AM                 |
	| AggregateRootId | C3579C12-C29B-4F65-8D83-B79AC5C85718 |
	| Sequence        | 4                                    |
	When I add the domain events to the store
	Then the following SQL statement should be run
	"""
	Insert into Event_Store (EventType, AggregateRootId, EventDate, Sequence) 
	Values ('SimpleCqrs.EventStore.SqlServer.Tests.SomethingHappenedEvent, SimpleCqrs.EventStore.SqlServer.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null', '8312E92C-DF1C-4970-A9D5-6414120C3CF7', '3/20/2010 3:01:04 AM' ,'2');
	"""
	And the following SQL statement should be run
	"""
	Insert into Event_Store (EventType, AggregateRootId, EventDate, Sequence) 
	Values ('SimpleCqrs.EventStore.SqlServer.Tests.SomethingElseHappenedEvent, SimpleCqrs.EventStore.SqlServer.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null', 'C3579C12-C29B-4F65-8D83-B79AC5C85718', '4/24/2010 3:01:04 AM' ,'4');
	"""