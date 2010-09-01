using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using System.Xml;
using NerdDinner.Models;

namespace NerdDinner.Controllers
{
    public class RssResult : FileResult
    {
        public List<Dinner> Dinners { get; set; }
        public string Title { get; set; }

        private Uri currentUrl;

        public RssResult() : base("application/rss+xml") { }

        public RssResult(List<Dinner> dinners, string title) :this()
        {
            this.Dinners = dinners;
            this.Title = title;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            currentUrl = context.RequestContext.HttpContext.Request.Url;
            base.ExecuteResult(context);
        }
        protected override void WriteFile(System.Web.HttpResponseBase response)
        {
            var items = new List<SyndicationItem>();

            foreach (Dinner d in this.Dinners)
            {
                string contentString = String.Format("{0} with {1} on {2:MMM dd, yyyy} at {3}. Where: {4}, {5}",
                            d.Description, d.HostedBy, d.EventDate, d.EventDate.ToShortTimeString(), d.Address, d.Country);
                
                var item = new SyndicationItem(
                    title: d.Title,
                    content: contentString,
                    itemAlternateLink: new Uri("http://nrddnr.com/" + d.DinnerID),
                    id: "http://nrddnr.com/" + d.DinnerID,
                    lastUpdatedTime: d.EventDate.ToUniversalTime()
                    );
                item.PublishDate = d.EventDate.ToUniversalTime();
                item.Summary = new TextSyndicationContent(contentString, TextSyndicationContentKind.Plaintext);
                items.Add(item);
            }

            SyndicationFeed feed = new SyndicationFeed(
                this.Title,
                this.Title, /* Using Title also as Description */
                currentUrl, 
                items);

            Rss20FeedFormatter formatter = new Rss20FeedFormatter(feed);

            using (XmlWriter writer = XmlWriter.Create(response.Output))
            {
                formatter.WriteTo(writer);
            }

        }
    }
}
