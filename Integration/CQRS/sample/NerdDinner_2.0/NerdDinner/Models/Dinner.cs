using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace NerdDinner.Models
{
    [Bind(Include = "Title,Description,EventDate,Address,Country,ContactPhone,Latitude,Longitude")]
    [MetadataType(typeof(DinnerValidation))]
    partial class Dinner
    {
        public bool IsHostedBy(string name)
        {
            return HostedBy == name;
        }

        public bool IsUserRegistered(string name)
        {
            var rsvpReadModel = new RsvpReadModel();
            return rsvpReadModel.IsUserRegistered(DinnerId, name);
        }

        public int GetRsvpCount()
        {
            var rsvpReadModel = new RsvpReadModel();
            return rsvpReadModel.GetNumberOfRsvpsForDinner(DinnerId);
        }

        public IEnumerable<Rsvp> GetRsvps()
        {
            var rsvpReadModel = new RsvpReadModel();
            return rsvpReadModel.GetRsvpsForDinner(DinnerId);
        }
    }

    public class DinnerValidation
    {
        [HiddenInput(DisplayValue = false)]
        public Guid DinnerId { get; set; }

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