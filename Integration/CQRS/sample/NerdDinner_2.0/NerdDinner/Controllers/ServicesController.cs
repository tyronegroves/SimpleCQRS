using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NerdDinner.Models;
using NerdDinner.Helpers;
using DDay.iCal.Components;
using NerdDinner.Services;
using System.ComponentModel;

namespace NerdDinner.Controllers
{
    [HandleErrorWithELMAH]
    public class ServicesController : Controller
    {
        private readonly DinnerReadModel dinnerReadModel = new DinnerReadModel();

        [OutputCache(VaryByParam = "none", Duration = 300)]
        public ActionResult RSS()
        {
            var dinners = dinnerReadModel.FindUpcomingDinners();

            if (dinners == null)
                return View("NotFound");

            return new RssResult(dinners.ToList(), "Upcoming Nerd Dinners");
        }

        [OutputCache(VaryByParam = "none", Duration = 300)]
        public ActionResult iCalFeed()
        {
            var dinners = dinnerReadModel.FindUpcomingDinners();

            if (dinners == null)
                return View("NotFound");

            return new iCalResult(dinners.ToList(), "NerdDinners.ics");
        }

        public ActionResult iCal(Guid id)
        {
            Dinner dinner = dinnerReadModel.GetDinnerById(id);

            if (dinner == null)
                return View("NotFound");

            return new iCalResult(dinner, "NerdDinner.ics");
        }

        public ActionResult Flair([DefaultValue("html")]string format)
        {
            string SourceIP = string.IsNullOrEmpty(Request.ServerVariables["HTTP_X_FORWARDED_FOR"]) ?
                Request.ServerVariables["REMOTE_ADDR"] :
                Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            var location = GeolocationService.HostIpToPlaceName(SourceIP);
            var dinners = dinnerReadModel.
                FindDinnerByLocation(location.Position.Lat, location.Position.Long).
                OrderByDescending(p => p.EventDate).Take(3);

            // Select the view we'll return. Using a switch because we'll add in JSON and other formats later.
            // Will probably extract or refactor this.
            string view;
            switch (format.ToLower())
            {
                case "javascript":
                    view = "JavascriptFlair";
                    break;
                default:
                    view = "Flair";
                    break;
            }

            return View(
                view,
                new FlairViewModel
                {
                    Dinners = dinners.ToList(),
                    LocationName = string.IsNullOrEmpty(location.City) ? "you" : String.Format("{0}, {1}", location.City, location.RegionName)
                }
            );
        }
    }
}
