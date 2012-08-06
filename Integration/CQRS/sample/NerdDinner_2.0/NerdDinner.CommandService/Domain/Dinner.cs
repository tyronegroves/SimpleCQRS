using System;
using System.Collections.Generic;
using NerdDinner.Commands;
using NerdDinner.CommandService.Events;
using SimpleCqrs.Domain;

namespace NerdDinner.CommandService.Domain
{
    public class Dinner : AggregateRoot
    {
        private List<Guid> rsvpAttendeeIds = new List<Guid>();
        private string title;
        private string description;
        private DateTime eventDate;
        private string contactPhone;
        private DinnerHost dinnerHost;
        private Location location;
        private bool cancelled;

        public Dinner()
        {
        }

        public Dinner(Guid dinnerId, DateTime eventDate, string title, string description, string contactPhone, DinnerHost host, Location location)
        {
            Apply(new DinnerCreatedEvent
                      {
                          DinnerId = dinnerId,
                          DinnerDate = eventDate,
                          Title = title,
                          Description = description,
                          ContactPhone = contactPhone
                      });
            Apply(new DinnerLocationSetEvent
                      {
                          Address = location.Address,
                          Country = location.Country,
                          Latitude = location.Latitude,
                          Longitude = location.Longitude
                      });
            Apply(new DinnerHostAssignedEvent {HostedById = host.HostedById, HostedBy = host.HostedBy});
            Apply(new DinnerRsvpAddedEvent {AttendeeId = host.HostedById, AttendeeName = host.HostedBy});
        }

        public void RegisterRsvp(Guid attendeeId, string attendeeName)
        {
            if(!rsvpAttendeeIds.Contains(attendeeId))
                Apply(new DinnerRsvpAddedEvent {AttendeeId = attendeeId, AttendeeName = attendeeName});
        }

        public void Edit(DateTime newEventDate, string newTitle, string newDescription, string newContactPhone,
                         DinnerHost newDinnerHost, Location newLocation)
        {
            if(title != newTitle || description != newDescription)
                Apply(new DinnerChangedEvent {NewTitle = newTitle, NewDescription = newDescription, PreviousTitle = title, PreviousDescription = description});

            if(eventDate != newEventDate)
                Apply(new DinnerEventDateChangedEvent {NewEventDate = newEventDate, PreviousEventDate = eventDate});

            if(contactPhone != newContactPhone)
                Apply(new DinnerContactPhoneChangedEvent {NewContactPhone = newContactPhone, PreviousContactPhone = contactPhone});

            if(!dinnerHost.Equals(newDinnerHost))
                Apply(new DinnerHostChangedEvent {HostedById = newDinnerHost.HostedById, HostedBy = newDinnerHost.HostedBy});

            if(!location.Equals(newLocation))
                Apply(new DinnerLocationChangedEvent {Address = location.Address, Country = location.Country, Latitude = location.Latitude, Longitude = location.Longitude});
        }

        public void Cancel()
        {
            if(!cancelled)
                Apply(new DinnerCancelledEvent());
        }

        protected void OnDinnerCreated(DinnerCreatedEvent domainEvent)
        {
            Id = domainEvent.DinnerId;
            eventDate = domainEvent.EventDate;
            contactPhone = domainEvent.ContactPhone;
        }

        protected void OnDinnerChanged(DinnerChangedEvent domainEvent)
        {
            title = domainEvent.NewTitle;
            description = domainEvent.NewDescription;
        }

        protected void OnDinnerEventDateChanged(DinnerEventDateChangedEvent domainEvent)
        {
            eventDate = domainEvent.NewEventDate;
        }

        protected void OnDinnerContactPhoneChanged(DinnerContactPhoneChangedEvent domainEvent)
        {
            contactPhone = domainEvent.NewContactPhone;
        }

        protected void OnDinnerHostChanged(DinnerHostChangedEvent domainEvent)
        {
            dinnerHost = new DinnerHost {HostedBy = domainEvent.HostedBy, HostedById = domainEvent.HostedById};
        }

        protected void OnDinnerLocationChanged(DinnerLocationChangedEvent domainEvent)
        {
            location = new Location
                           {
                               Address = domainEvent.Address,
                               Country = domainEvent.Country,
                               Latitude = domainEvent.Latitude,
                               Longitude = domainEvent.Longitude
                           };
        }

        protected void OnDinnerHostAssigned(DinnerHostAssignedEvent domainEvent)
        {
            dinnerHost = new DinnerHost {HostedBy = domainEvent.HostedBy, HostedById = domainEvent.HostedById};
        }

        protected void OnDinnerLocationSet(DinnerLocationSetEvent domainEvent)
        {
            location = new Location
                           {
                               Address = domainEvent.Address,
                               Country = domainEvent.Country,
                               Latitude = domainEvent.Latitude,
                               Longitude = domainEvent.Longitude
                           };
        }

        protected void OnDinnerRsvpAdded(DinnerRsvpAddedEvent domainEvent)
        {
            if(rsvpAttendeeIds == null)
                rsvpAttendeeIds = new List<Guid>();

            rsvpAttendeeIds.Add(domainEvent.AttendeeId);
        }

        protected void OnDinnerCancelled(DinnerCancelledEvent domainEvent)
        {
            cancelled = true;
        }
    }
}