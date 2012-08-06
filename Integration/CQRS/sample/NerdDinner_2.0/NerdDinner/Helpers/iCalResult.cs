using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using DDay.iCal.Components;
using NerdDinner.Helpers;
using DDay.iCal;
using DDay.iCal.Serialization;
using NerdDinner.Models;

namespace NerdDinner.Controllers
{
    class iCalResult : FileResult
    {
        public List<Dinner> Dinners { get; set; }

        public iCalResult(string filename) : base("text/calendar")
        {
            this.FileDownloadName = filename;
        }

        public iCalResult(List<Dinner> dinners, string filename) : this(filename)
        {
            this.Dinners = dinners;
        }

        public iCalResult(Dinner dinner, string filename) : this(filename)
        {
            this.Dinners = new List<Dinner>();
            this.Dinners.Add(dinner);
        }

        protected override void WriteFile(System.Web.HttpResponseBase response)
        {
            iCalendar iCal = new iCalendar();
            foreach (Dinner d in this.Dinners)
            {
                try
                {
                    Event e = CalendarHelpers.DinnerToEvent(d, iCal);
                    iCal.Events.Add(e);
                }
                catch (ArgumentOutOfRangeException)
                { 
                    //Swallow folks that have dinners in 9999. 
                }
            }

            iCalendarSerializer serializer = new iCalendarSerializer(iCal);
            string result = serializer.SerializeToString();
            response.ContentEncoding = Encoding.UTF8;
            response.Write(result);
        }

    }
}
