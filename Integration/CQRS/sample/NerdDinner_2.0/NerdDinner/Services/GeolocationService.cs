using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Caching;
using NerdDinner.Helpers;
using System.Xml.Linq;

namespace NerdDinner.Services
{
    public class GeolocationService
    {

        public static LatLong PlaceOrZipToLatLong(string placeOrZip)
        {
            ObjectCache cache = MemoryCache.Default;

            string url = "http://ws.geonames.org/postalCodeSearch?{0}={1}&maxRows=1&style=SHORT";
            url = String.Format(url, placeOrZip.IsNumeric() ? "postalcode" : "placename", placeOrZip);

            var result = cache[placeOrZip] as XDocument;
            if (result == null)
            {
                result = XDocument.Load(url);
                cache.Add(placeOrZip, result,
                    new CacheItemPolicy() { SlidingExpiration = TimeSpan.FromDays(1) });
            }

            var ll = (from x in result.Descendants("code")
                           select new LatLong
                           {
                               Lat = (float)x.Element("lat"),
                               Long = (float)x.Element("lng")
                       }).First();

            return ll;
        }

        public static LocationInfo HostIpToPlaceName(string ip)
        {
            if (ip == "127.0.0.1")
            {
                ip = "71.117.141.83";
                //return string.Empty;
            }

            string url = "http://ipinfodb.com/ip_query.php?ip={0}&timezone=false";
            url = String.Format(url, ip);

            var result = XDocument.Load(url);

            var location = (from x in result.Descendants("Response")
                            select new LocationInfo
                            {
                                City = (string)x.Element("City"),
                                RegionName = (string)x.Element("RegionName"),
                                Country = (string)x.Element("CountryName"),
                                ZipPostalCode = (string)x.Element("CountryName"),
                                Position = new LatLong
                                {
                                    Lat = (float)x.Element("Latitude"),
                                    Long = (float)x.Element("Longitude")
                                }
                            }).First();

            return location;
        }
    }

    public class LatLong
    {
        public float Lat { get; set; }
        public float Long { get; set; }
    }

    public class LocationInfo
    {
        public string Country { get; set; }
        public string RegionName { get; set; }
        public string City { get; set; }
        public string ZipPostalCode { get; set; }
        public LatLong Position { get; set; }
    }
}