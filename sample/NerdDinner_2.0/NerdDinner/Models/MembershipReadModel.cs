using System;
using System.Linq;

namespace NerdDinner.Models
{
    public class MembershipReadModel
    {
        private readonly NerdDinnerEntities db = new NerdDinnerEntities();

        public MembershipReadModel()
        {
            MinPasswordLength = 6;
        }

        public int MinPasswordLength { get; private set; }

        public string GetCanonicalUsername(string userName)
        {
            return (from membership in db.Memberships
                    where membership.UserName == userName
                    select membership.CanonicalUsername).FirstOrDefault();
        }

        public bool ValidateUser(string userName, string password)
        {
            return db.Memberships.Any(membership => membership.UserName == userName && membership.Password == password);
        }

        public Guid GetUserIdByUserName(string userName)
        {
            return (from membership in db.Memberships
                    where membership.UserName == userName
                    select membership.UserId).FirstOrDefault();
        }
    }
}