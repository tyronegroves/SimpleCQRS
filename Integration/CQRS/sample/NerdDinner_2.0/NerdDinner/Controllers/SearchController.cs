using System;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using NerdDinner.Helpers;
using NerdDinner.Models;
using NerdDinner.Services;

namespace NerdDinner.Controllers
{

    public class JsonDinner
    {
        public Guid DinnerId { get; set; }
        public DateTime EventDate { get; set; }
        public string Title { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Description { get; set; }
        public string RSVPCount { get; set; }
        public string Url { get; set; }
    }

    [HandleErrorWithELMAH]
    public class SearchController : Controller
    {
        private readonly DinnerReadModel dinnerReadModel = new DinnerReadModel();

        [HttpPost]
        public ActionResult SearchByLocation(float latitude, float longitude)
        {
            var dinners = dinnerReadModel.FindDinnerByLocation(latitude, longitude);

            var jsonDinners = from dinner in dinners.AsEnumerable()
                              select JsonDinnerFromDinner(dinner);

            return Json(jsonDinners.ToList());
        }

        [HttpPost]
        public ActionResult SearchByPlaceNameOrZip(string placeOrZip)
        {
            if (String.IsNullOrEmpty(placeOrZip)) return null;
            var location = GeolocationService.PlaceOrZipToLatLong(placeOrZip);

            var dinners = dinnerReadModel
                    .FindDinnerByLocation(location.Lat, location.Long)
                    .OrderByDescending(p => p.EventDate);

            return View("Results", new PaginatedList<Dinner>(dinners, 0, 20));
        }

        [HttpPost]
        public ActionResult GetMostPopularDinners(int? limit)
        {
            var dinners = dinnerReadModel.FindPopularDinners();

            // Default the limit to 40, if not supplied.
            if (!limit.HasValue)
                limit = 40;

            var mostPopularDinners = from dinner in dinners
                                     orderby dinner.RsvpCount descending
                                     select dinner;

            var jsonDinners =
                mostPopularDinners.Take(limit.Value).AsEnumerable()
                .Select(JsonDinnerFromPopularDinner);

            return Json(jsonDinners.ToList());
        }

        private static JsonDinner JsonDinnerFromDinner(Dinner dinner)
        {
            return new JsonDinner
            {
                DinnerId = dinner.DinnerId,
                EventDate = dinner.EventDate,
                Latitude = dinner.Latitude,
                Longitude = dinner.Longitude,
                Title = dinner.Title,
                Description = dinner.Description,
                RSVPCount = dinner.GetRsvpCount().ToString(),

                //TODO: Need to mock this out for testing...
                //Url = Url.RouteUrl("PrettyDetails", new { Id = dinner.DinnerID } )
                Url = dinner.DinnerId.ToString()
            };
        }

        private static JsonDinner JsonDinnerFromPopularDinner(PopularDinner popularDinner)
        {
            return new JsonDinner
            {
                DinnerId = popularDinner.DinnerId,
                EventDate = popularDinner.EventDate,
                Latitude = popularDinner.Latitude.GetValueOrDefault(),
                Longitude = popularDinner.Longitude.GetValueOrDefault(),
                Title = popularDinner.Title,
                Description = popularDinner.Description,
                RSVPCount = popularDinner.RsvpCount.GetValueOrDefault().ToString(),

                //TODO: Need to mock this out for testing...
                //Url = Url.RouteUrl("PrettyDetails", new { Id = dinner.DinnerID } )
                Url = popularDinner.DinnerId.ToString()
            };
        }
    }
}