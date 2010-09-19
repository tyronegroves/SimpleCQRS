using System;
using System.Data.Objects.DataClasses;
using System.Linq;

namespace NerdDinner.Models
{
    public class DinnerReadModel
    {
        private readonly NerdDinnerEntities db = new NerdDinnerEntities();

        public IQueryable<Dinner> FindDinnersByText(string term)
        {
            return Queryable.Where(db.Dinners, d => d.Title.Contains(term)
                                                    || d.Description.Contains(term)
                                                    || d.HostedBy.Contains(term));
        }

        public IQueryable<Dinner> FindUpcomingDinners()
        {
            return FindUpcomingDinners(DateTime.Now);
        }

        public IQueryable<Dinner> FindUpcomingDinners(DateTime eventDate)
        {
            return from dinner in db.Dinners
                   where dinner.EventDate >= eventDate
                   orderby dinner.EventDate descending 
                   select dinner;
        }

        public Dinner GetDinnerById(Guid dinnerId)
        {
            return Queryable.FirstOrDefault(db.Dinners, dinner => dinner.DinnerId == dinnerId);
        }

        public bool DinnerIsHostedBy(Guid dinnerId, string hostName)
        {
            return Queryable.Any((from dinner in db.Dinners
                                  where dinner.HostedBy == hostName
                                  where dinner.DinnerId == dinnerId
                                  select dinner));
        }

        public IQueryable<Dinner> FindAllDinnersByUserId(Guid userId)
        {
            var myDinnerIds = db.Dinners.Where(dinner => dinner.HostedById == userId).Select(dinner => dinner.DinnerId);
            var rsvpDinnerIds = db.Rsvps.Where(rsvp => rsvp.AttendeeId == userId).Select(dinner => dinner.DinnerId);

            var userDinnerIds = myDinnerIds.Union(rsvpDinnerIds).Distinct();
            return from dinner in db.Dinners
                   where userDinnerIds.Contains(dinner.DinnerId)
                   select dinner;

            //var userDinners = from dinner in dinnerRepository.FindAllDinners()
            //                  where
            //                    (
            //                    String.Equals((dinner.HostedById ?? dinner.HostedBy), nerd.Name)
            //                        ||
            //                    dinner.RSVPs.Any(r => r.AttendeeNameId == nerd.Name || (r.AttendeeNameId == null && r.AttendeeName == nerd.Name))
            //                    )
            //                  orderby dinner.EventDate
            //                  select dinner;))))))
        }

        public IQueryable<PopularDinner> FindPopularDinners()
        {
            return from popularDinner in db.PopularDinners
                   orderby popularDinner.RsvpCount descending
                   select popularDinner;
        }

        public bool DinnerExists(Guid dinnerId)
        {
            return (from dinner in db.Dinners
                   where dinner.DinnerId == dinnerId
                   select dinner).Any();
        }

        public IQueryable<Dinner> FindDinnerByLocation(float latitude, float longitude)
        {
            return from dinner in db.Dinners
                   join i in NearestDinners(latitude, longitude)
                   on dinner.DinnerId equals i.DinnerId
                   select dinner;
        }

        [EdmFunction("NerdDinnerModel.Store", "DistanceBetween")]
        public static double DistanceBetween(double lat1, double long1, double lat2, double long2)
        {
            throw new NotImplementedException("Only call through LINQ expression");
        }

        public IQueryable<Dinner> NearestDinners(double latitude, double longitude)
        {
            return from d in db.Dinners
                   where DistanceBetween(latitude, longitude, d.Latitude, d.Longitude) < 100
                   select d;
        }
    }
}