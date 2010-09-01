using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace NerdDinner.Models
{
    [Bind(Include = "Title,Description,EventDate,Address,Country,ContactPhone,Latitude,Longitude")]
    [MetadataType(typeof(Dinner_Validation))]
    public partial class Dinner
    {
        public bool IsHostedBy(string userName)
        {
            return String.Equals(HostedById ?? HostedBy, userName, StringComparison.Ordinal);
        }

        public bool IsUserRegistered(string userName)
        {
            return RSVPs.Any(r => r.AttendeeNameId == userName || (r.AttendeeNameId == null && r.AttendeeName == userName));
        }

        [UIHint("LocationDetail")]
        public LocationDetail Location
        {
            get
            {
                return new LocationDetail() { Latitude = this.Latitude, Longitude = this.Longitude, Title = this.Title, Address = this.Address };
            }
            set
            {
                this.Latitude = value.Latitude;
                this.Longitude = value.Longitude;
                this.Title = value.Title;
                this.Address = value.Address;
            }
        }
    }

    public class LocationDetail
    {
        public double Latitude;
        public double Longitude;
        public string Title;
        public string Address;
    }

    public class Dinner_Validation
    {
        [HiddenInput(DisplayValue = false)]
        public int DinnerID { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(50, ErrorMessage = "Title may not be longer than 50 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(265, ErrorMessage = "Description may not be longer than 256 characters")]
        public string Description { get; set; }

        public string HostedById { get; set; }

        [StringLength(256, ErrorMessage = "Hosted By name may not be longer than 20 characters")]
        public string HostedBy { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(50, ErrorMessage = "Address may not be longer than 50 characters")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Country is required")]
        [StringLength(30, ErrorMessage = "Country may not be longer than 30 characters")]
        [UIHint("CountryDropDown")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Contact phone is required")]
        [StringLength(20, ErrorMessage = "Contact phone may not be longer than 20 characters")]
        public string ContactPhone { get; set; }

        [HiddenInput(DisplayValue = false)]
        public double Latitude { get; set; }

        [HiddenInput(DisplayValue = false)]
        public double Longitude { get; set; }
    }
}