using System.Linq;

namespace NerdDinner.Models
{
    public class MembershipReadModel
    {
        private readonly NerdDinnerEntities db = new NerdDinnerEntities();

        public int GetMinPasswordLength()
        {
            return db.MinPasswordLengths.First().MinLength;
        }

        public string GetCanonicalUsername(string userName)
        {
            return db.CanonicalUsernames.FirstOrDefault(cn => cn.Username == userName).LoweredUserName;
        }

        public bool ValidateUser(string userName, string password)
        {
            return db.ValidateUsers.Any(vu => vu.Username == userName && vu.Password == password);
        }
    }
}