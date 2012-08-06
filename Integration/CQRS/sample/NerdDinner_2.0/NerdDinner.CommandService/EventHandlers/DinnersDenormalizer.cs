using System.Linq;
using NerdDinner.CommandService.Events;
using NerdDinner.CommandService.Models;
using SimpleCqrs.Eventing;

namespace NerdDinner.CommandService.EventHandlers
{
    public class DinnersDenormalizer :
        IHandleDomainEvents<DinnerCreatedEvent>,
        IHandleDomainEvents<DinnerCancelledEvent>,
        IHandleDomainEvents<DinnerHostAssignedEvent>,
        IHandleDomainEvents<DinnerLocationSetEvent>,
        IHandleDomainEvents<DinnerEventDateChangedEvent>,
        IHandleDomainEvents<DinnerContactPhoneChangedEvent>,
        IHandleDomainEvents<DinnerChangedEvent>,
        IHandleDomainEvents<DinnerLocationChangedEvent>
    {
        private readonly NerdDinnerEntities db = new NerdDinnerEntities();

        public void Handle(DinnerCreatedEvent domainEvent)
        {
            var dinnerReadModel = DinnerReadModel.CreateDinnerReadModel(domainEvent.DinnerId, domainEvent.Title, domainEvent.DinnerDate, domainEvent.Description);
            dinnerReadModel.ContactPhone = domainEvent.ContactPhone;

            db.AddToDinnerReadModels(dinnerReadModel);
            db.SaveChanges();
        }

        public void Handle(DinnerHostAssignedEvent domainEvent)
        {
            var dinnerReadModel = GetDinnerReadModel(domainEvent);
            if(dinnerReadModel == null) return;

            dinnerReadModel.HostedBy = domainEvent.HostedBy;
            dinnerReadModel.HostedById = domainEvent.HostedById;

            db.SaveChanges();
        }

        public void Handle(DinnerLocationSetEvent domainEvent)
        {
            var dinnerReadModel = GetDinnerReadModel(domainEvent);
            if(dinnerReadModel == null) return;

            dinnerReadModel.Address = domainEvent.Address;
            dinnerReadModel.Country = domainEvent.Country;
            dinnerReadModel.Latitude = domainEvent.Latitude;
            dinnerReadModel.Longitude = domainEvent.Longitude;

            db.SaveChanges();
        }

        public void Handle(DinnerCancelledEvent domainEvent)
        {
            var dinnerReadModel = GetDinnerReadModel(domainEvent);
            if(dinnerReadModel == null) return;

            db.DinnerReadModels.DeleteObject(dinnerReadModel);
            db.SaveChanges();
        }

        public void Handle(DinnerEventDateChangedEvent domainEvent)
        {
            var dinnerReadModel = GetDinnerReadModel(domainEvent);
            if(dinnerReadModel == null) return;
            dinnerReadModel.EventDate = domainEvent.NewEventDate;

            db.SaveChanges();
        }

        public void Handle(DinnerContactPhoneChangedEvent domainEvent)
        {
            var dinnerReadModel = GetDinnerReadModel(domainEvent);
            if(dinnerReadModel == null) return;
            dinnerReadModel.ContactPhone = domainEvent.NewContactPhone;

            db.SaveChanges();
        }

        public void Handle(DinnerChangedEvent domainEvent)
        {
            var dinnerReadModel = GetDinnerReadModel(domainEvent);
            if(dinnerReadModel == null) return;
            dinnerReadModel.Title = domainEvent.NewTitle;
            dinnerReadModel.Description = domainEvent.NewDescription;

            db.SaveChanges();
        }

        public void Handle(DinnerLocationChangedEvent domainEvent)
        {
            var dinnerReadModel = GetDinnerReadModel(domainEvent);
            if(dinnerReadModel == null) return;

            dinnerReadModel.Address = domainEvent.Address;
            dinnerReadModel.Country = domainEvent.Country;
            dinnerReadModel.Latitude = domainEvent.Latitude;
            dinnerReadModel.Longitude = domainEvent.Longitude;

            db.SaveChanges();
        }

        private DinnerReadModel GetDinnerReadModel(DomainEvent domainEvent)
        {
            return db.DinnerReadModels.FirstOrDefault(d => d.DinnerId == domainEvent.AggregateRootId);
        }
    }
}