using System;
using System.Globalization;
using System.Web.Mvc;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using DotNetOpenAuth.OpenId.RelyingParty;
using NerdDinner.Helpers;
using NerdDinner.Models;

namespace NerdDinner.Controllers
{
		[HandleErrorWithELMAH]
		public class RSVPController : Controller
		{

        IDinnerRepository dinnerRepository;

        private static OpenIdRelyingParty relyingParty = new OpenIdRelyingParty(null);

        //
        // Dependency Injection enabled constructors

        public RSVPController()
            : this(new DinnerRepository()) {
        }

        public RSVPController(IDinnerRepository repository) {
            dinnerRepository = repository;
        }

        //
        // AJAX: /Dinners/Register/1

        [Authorize, HttpPost]
        public ActionResult Register(int id) {

            Dinner dinner = dinnerRepository.GetDinner(id);

            if (!dinner.IsUserRegistered(User.Identity.Name)) {

                RSVP rsvp = new RSVP();
                NerdIdentity nerd = (NerdIdentity)User.Identity;
                rsvp.AttendeeNameId = nerd.Name;
                rsvp.AttendeeName = nerd.FriendlyName;

                dinner.RSVPs.Add(rsvp);
                dinnerRepository.Save();
            }

            return Content("Thanks - we'll see you there!");
        }

        //
        // GET: /RSVP/RsvpBegin

        public ActionResult RsvpBegin(string identifier, int id)
        {
            Uri returnTo = new Uri(new Uri(Realm.AutoDetect), Url.Action("RsvpFinish"));
            IAuthenticationRequest request = relyingParty.CreateRequest(identifier, Realm.AutoDetect, returnTo);
            request.SetUntrustedCallbackArgument("DinnerId", id.ToString(CultureInfo.InvariantCulture));
            request.AddExtension(new ClaimsRequest { Email = DemandLevel.Require, FullName = DemandLevel.Request });
            return request.RedirectingResponse.AsActionResult();
        }

        //
        // GET: /RSVP/RsvpBegin
        // POST: /RSVP/RsvpBegin

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post), ValidateInput(false)]
        public ActionResult RsvpFinish()
        {
            IAuthenticationResponse response = relyingParty.GetResponse();
            if (response == null)
            {
                return RedirectToAction("Index");
            }

            if (response.Status == AuthenticationStatus.Authenticated)
            {
                var dinnerRepository = new DinnerRepository();
                int id = int.Parse(response.GetUntrustedCallbackArgument("DinnerId"));
                Dinner dinner = dinnerRepository.GetDinner(id);

                // The alias we're getting here is NOT a secure identifier, but a friendly one,
                // which is all we need for this scenario.
                string alias = response.FriendlyIdentifierForDisplay;
                var sreg = response.GetExtension<ClaimsResponse>();
                if (sreg != null && sreg.MailAddress != null)
                {
                    alias = sreg.MailAddress.User;
                }

                // NOTE: The alias we've generated for this user isn't guaranteed to be unique.
                // Need to trim to 30 characters because that's the max for Attendee names.
                if (!dinner.IsUserRegistered(alias))
                {
                    RSVP rsvp = new RSVP();
                    rsvp.AttendeeName = alias;
                    rsvp.AttendeeNameId = response.ClaimedIdentifier;

                    dinner.RSVPs.Add(rsvp);
                    dinnerRepository.Save();
                }
            }

            return RedirectToAction("Details", "Dinners", new { id = response.GetUntrustedCallbackArgument("DinnerId") });
        }

        // GET: /RSVP/RsvpTwitterBegin

        public ActionResult RsvpTwitterBegin(int id)
        {
            Uri callback = new Uri(new Uri(Realm.AutoDetect), Url.Action("RsvpTwitterFinish", new { id = id }));
            return TwitterConsumer.StartSignInWithTwitter(false, callback).AsActionResult();
        }

        // GET: /RSVP/RsvpTwitterFinish

        public ActionResult RsvpTwitterFinish(int id)
        {
            string screenName;
            int userId;
            if (TwitterConsumer.TryFinishSignInWithTwitter(out screenName, out userId))
            {
                var dinnerRepository = new DinnerRepository();
                Dinner dinner = dinnerRepository.GetDinner(id);

                // NOTE: The alias we've generated for this user isn't guaranteed to be unique.
                string alias = "@" + screenName;
                if (!dinner.IsUserRegistered(alias))
                {
                    RSVP rsvp = new RSVP();
                    rsvp.AttendeeName = alias;

                    dinner.RSVPs.Add(rsvp);
                    dinnerRepository.Save();
                }
            }

            return RedirectToAction("Details", "Dinners", new { id = id });
        }
    }
}
