using System;
using System.Linq;

namespace NerdDinner.CommandService.Models
{
    public interface IUserService
    {
        bool UserIdExists(Guid hostedById);
    }

    public class UserService : IUserService
    {
        private readonly NerdDinnerEntities db = new NerdDinnerEntities();

        public bool UserIdExists(Guid hostedById)
        {
            return db.UserReadModels.Any(membership => membership.UserId == hostedById);
        }
    }
}