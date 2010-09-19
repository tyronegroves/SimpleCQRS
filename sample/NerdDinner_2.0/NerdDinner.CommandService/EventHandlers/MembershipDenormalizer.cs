using System.Linq;
using NerdDinner.CommandService.Events;
using NerdDinner.CommandService.Models;
using SimpleCqrs.Eventing;

namespace NerdDinner.CommandService.EventHandlers
{
    public class MembershipDenormalizer : IHandleDomainEvents<UserCreatedEvent>,
                                          IHandleDomainEvents<UserPasswordChangedEvent>
    {
        private readonly NerdDinnerEntities db = new NerdDinnerEntities();

        public void Handle(UserCreatedEvent domainEvent)
        {
            var userReadModel = new UserReadModel
                                    {
                                        UserId = domainEvent.UserId,
                                        UserName = domainEvent.UserName,
                                        Password = domainEvent.Password,
                                        Email = domainEvent.Email,
                                        CanonicalUsername = domainEvent.UserName.ToLower()
                                    };
            db.AddToUserReadModels(userReadModel);
            db.SaveChanges();
        }

        public void Handle(UserPasswordChangedEvent domainEvent)
        {
            var userReadModel = db.UserReadModels.FirstOrDefault(m => m.UserId == domainEvent.UserId);
            if(userReadModel == null) return;

            userReadModel.Password = domainEvent.NewPassword;
            db.SaveChanges();
        }
    }
}