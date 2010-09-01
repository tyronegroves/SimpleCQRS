using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Objects.DataClasses;
using System.Data;

namespace NerdDinner.Models
{

    public class DinnerRepository : NerdDinner.Models.IDinnerRepository
    {

        NerdDinnerEntities db = new NerdDinnerEntities();

        //
        // Query Methods

        public IQueryable<Dinner> FindDinnersByText(string q)
        {
            return db.Dinners.Where(d => d.Title.Contains(q)
                            || d.Description.Contains(q)
                            || d.HostedBy.Contains(q));
        }

        public IQueryable<Dinner> FindAllDinners()
        {
            return db.Dinners;
        }

        public IQueryable<Dinner> FindUpcomingDinners()
        {
            return from dinner in FindAllDinners()
                   where dinner.EventDate >= DateTime.Now
                   orderby dinner.EventDate
                   select dinner;
        }

        public IQueryable<Dinner> FindByLocation(float latitude, float longitude)
        {
            var dinners = from dinner in FindUpcomingDinners()
                          join i in NearestDinners(latitude, longitude)
                          on dinner.DinnerID equals i.DinnerID
                          select dinner;

            return dinners;
        }

        public Dinner GetDinner(int id)
        {
            return db.Dinners.SingleOrDefault(d => d.DinnerID == id);
        }

        //
        // Insert/Delete Methods

        public void Add(Dinner dinner)
        {
            db.Dinners.AddObject(dinner);
        }

        public void Delete(Dinner dinner)
        {
            foreach (RSVP rsvp in dinner.RSVPs.ToList())
                db.RSVPs.DeleteObject(rsvp);
            db.Dinners.DeleteObject(dinner);
        }

        //
        // Persistence 

        public void Save()
        {
            db.SaveChanges();
        }


        // Helper Methods

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
