using System.Linq;
using NerdDinner.CommandService.Events;
using NerdDinner.CommandService.Models;
using SimpleCqrs.Eventing;

namespace NerdDinner.CommandService.EventHandlers
{
    public class PopularDinnersDenormalizer :
        IHandleDomainEvents<DinnerCreatedEvent>,
        IHandleDomainEvents<DinnerCancelledEvent>,
        IHandleDomainEvents<DinnerHostAssignedEvent>,
        IHandleDomainEvents<DinnerLocationSetEvent>,
        IHandleDomainEvents<DinnerEventDateChangedEvent>,
        IHandleDomainEvents<DinnerContactPhoneChangedEvent>,
        IHandleDomainEvents<DinnerChangedEvent>,
        IHandleDomainEvents<DinnerLocationChangedEvent>,
        IHandleDomainEvents<DinnerRsvpAddedEvent>
    {
        private readonly NerdDinnerEntities db = new NerdDinnerEntities();

        public void Handle(DinnerCreatedEvent domainEvent)
        {
            var popularDinnerReadModel = PopularDinnerReadModel.CreatePopularDinnerReadModel(domainEvent.DinnerId, domainEvent.Title, domainEvent.DinnerDate, domainEvent.Description);
            popularDinnerReadModel.ContactPhone = domainEvent.ContactPhone;
            popularDinnerReadModel.RsvpCount = 0;

            db.AddToPopularDinnerReadModels(popularDinnerReadModel);
            db.SaveChanges();
        }

        public void Handle(DinnerHostAssignedEvent domainEvent)
        {
            var popularDinnerReadModel = GetPopularDinnerReadModel(domainEvent);
            if (popularDinnerReadModel == null) return;

            popularDinnerReadModel.HostedBy = domainEvent.HostedBy;
            popularDinnerReadModel.HostedById = domainEvent.HostedById;

            db.SaveChanges();
        }

        public void Handle(DinnerLocationSetEvent domainEvent)
        {
            var popularDinnerReadModel = GetPopularDinnerReadModel(domainEvent);
            if (popularDinnerReadModel == null) return;

            popularDinnerReadModel.Address = domainEvent.Address;
            popularDinnerReadModel.Country = domainEvent.Country;
            popularDinnerReadModel.Latitude = domainEvent.Latitude;
            popularDinnerReadModel.Longitude = domainEvent.Longitude;

            db.SaveChanges();
        }

        public void Handle(DinnerCancelledEvent domainEvent)
        {
            var popularDinnerReadModel = GetPopularDinnerReadModel(domainEvent);
            if (popularDinnerReadModel == null) return;

            db.PopularDinnerReadModels.DeleteObject(popularDinnerReadModel);
            db.SaveChanges();
        }

        public void Handle(DinnerEventDateChangedEvent domainEvent)
        {
            var popularDinnerReadModel = GetPopularDinnerReadModel(domainEvent);
            if (popularDinnerReadModel == null) return;
           
            popularDinnerReadModel.EventDate = domainEvent.NewEventDate;
            db.SaveChanges();
        }

        public void Handle(DinnerContactPhoneChangedEvent domainEvent)
        {
            var popularDinnerReadModel = GetPopularDinnerReadModel(domainEvent);
            if (popularDinnerReadModel == null) return;
            
            popularDinnerReadModel.ContactPhone = domainEvent.NewContactPhone;
            db.SaveChanges();
        }

        public void Handle(DinnerChangedEvent domainEvent)
        {
            var popularDinnerReadModel = GetPopularDinnerReadModel(domainEvent);
            if (popularDinnerReadModel == null) return;

            popularDinnerReadModel.Title = domainEvent.NewTitle;
            popularDinnerReadModel.Description = domainEvent.NewDescription;

            db.SaveChanges();
        }

        public void Handle(DinnerLocationChangedEvent domainEvent)
        {
            var popularDinnerReadModel = GetPopularDinnerReadModel(domainEvent);
            if (popularDinnerReadModel == null) return;

            popularDinnerReadModel.Address = domainEvent.Address;
            popularDinnerReadModel.Country = domainEvent.Country;
            popularDinnerReadModel.Latitude = domainEvent.Latitude;
            popularDinnerReadModel.Longitude = domainEvent.Longitude;

            db.SaveChanges();
        }

        public void Handle(DinnerRsvpAddedEvent domainEvent)
        {
            var popularDinnerReadModel = db.PopularDinnerReadModels.FirstOrDefault(d => d.DinnerId == domainEvent.DinnerId);
            if(popularDinnerReadModel == null) return;

            popularDinnerReadModel.RsvpCount++;
            db.SaveChanges();
        }

        private PopularDinnerReadModel GetPopularDinnerReadModel(DomainEvent domainEvent)
        {
            return db.PopularDinnerReadModels.FirstOrDefault(d => d.DinnerId == domainEvent.AggregateRootId);
        }
    }
}