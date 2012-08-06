using System;
using System.Collections.Generic;
using System.Linq;

namespace NerdDinner.Models
{
    public class RsvpReadModel
    {
        private readonly NerdDinnerEntities db = new NerdDinnerEntities();

        public bool IsUserRegistered(Guid dinnerId, string name)
        {
            return (from rsvp in db.Rsvps
                    where rsvp.DinnerId == dinnerId
                    where rsvp.AttendeeName == name
                    select rsvp).Any();
        }

        public int GetNumberOfRsvpsForDinner(Guid dinnerId)
        {
            return db.Rsvps.Where(rsvp => rsvp.DinnerId == dinnerId).Count();
        }

        public IEnumerable<Rsvp> GetRsvpsForDinner(Guid dinnerId)
        {
            return db.Rsvps.Where(rsvp => rsvp.DinnerId == dinnerId);
        }
    }
}