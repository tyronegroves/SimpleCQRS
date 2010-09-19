using System;
using NerdDinner.CommandService.Events;
using SimpleCqrs.Domain;

namespace NerdDinner.CommandService.Domain
{
    public class User : AggregateRoot
    {
        public User()
        {    
        }

        public User(Guid userId, string userName, string password, string email)
        {
            Apply(new UserCreatedEvent {UserId = userId, UserName = userName, Password = password, Email = email});
        }

        public void ChangePassword(string newPassword)
        {
            Apply(new UserPasswordChangedEvent {NewPassword = newPassword});
        }

        protected void OnUserCreated(UserCreatedEvent domainEvent)
        {
            Id = domainEvent.AggregateRootId;
        }
    }
}