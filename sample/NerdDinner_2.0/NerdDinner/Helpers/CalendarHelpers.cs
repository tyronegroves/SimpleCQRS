using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NerdDinner.Models;
using DDay.iCal;
using DDay.iCal.Components;
using DDay.iCal.Serialization;
using System.Web.Mvc;
using DDay.iCal.DataTypes;

namespace NerdDinner.Helpers
{
    public static class CalendarHelpers
    {
        public static Event DinnerToEvent(Dinner dinner, iCalendar iCal)
        {
            string eventLink = "http://nrddnr.com/" + dinner.DinnerID;
            Event evt = iCal.Create<Event>();
            evt.Start = dinner.EventDate;
            evt.Duration = new TimeSpan(3, 0, 0);
            evt.Location = dinner.Address;
            evt.Summary = String.Format("{0} with {1}", dinner.Description, dinner.HostedBy);
            evt.AddContact(dinner.ContactPhone);
            evt.Geo = new Geo(dinner.Latitude, dinner.Longitude);
            evt.Url = eventLink;
            evt.Description = eventLink;
            return evt;
        }
    }
}