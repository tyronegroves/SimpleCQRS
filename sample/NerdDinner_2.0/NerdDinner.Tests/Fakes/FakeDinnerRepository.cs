using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NerdDinner.Models;

namespace NerdDinner.Tests.Fakes {

    public class FakeDinnerRepository : IDinnerRepository {

        private List<Dinner> dinnerList;

        public FakeDinnerRepository(List<Dinner> dinners) {
            dinnerList = dinners;
        }

        public IQueryable<Dinner> FindDinnersByText(string q)        {
            return dinnerList.AsQueryable().Where(d => d.Title.Contains(q)
                || d.Description.Contains(q)
                || d.HostedBy.Contains(q));
        }

        public IQueryable<Dinner> FindAllDinners() {
            return dinnerList.AsQueryable();
        }

        public IQueryable<Dinner> FindUpcomingDinners() {
            return (from dinner in dinnerList
                    where dinner.EventDate > DateTime.Now.AddDays(-1)
					orderby dinner.EventDate
                    select dinner).AsQueryable();
        }

        public IQueryable<Dinner> FindByLocation(float lat, float lon) {
            return (from dinner in dinnerList
                    where dinner.Latitude == lat && dinner.Longitude == lon
                    select dinner).AsQueryable();
        }

        public Dinner GetDinner(int id) {
            return dinnerList.SingleOrDefault(d => d.DinnerID == id);
        }

        public void Add(Dinner dinner) {
            dinnerList.Add(dinner);
        }

        public void Delete(Dinner dinner) {
            dinnerList.Remove(dinner);
        }

        public void Save() {
            foreach (Dinner dinner in dinnerList) {
                //TODO: Remove this
                //if (!dinner.IsValid)
                //    throw new ApplicationException("Rule violations");
            }
        }
    }
}
