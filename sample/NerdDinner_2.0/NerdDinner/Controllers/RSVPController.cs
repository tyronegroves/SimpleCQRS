using System;
using System.Globalization;
using System.Web.Mvc;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using DotNetOpenAuth.OpenId.RelyingParty;
using NerdDinner.Commands;
using NerdDinner.Helpers;
using NerdDinner.Models;
using NerdDinner.Services;

namespace NerdDinner.Controllers
{
    [HandleErrorWithELMAH]
    public class RSVPController : Controller
    {
        private readonly DinnerReadModel dinnerReadModel = new DinnerReadModel();
        private readonly ICommandService commandService = new CommandServiceClient();
        private static readonly OpenIdRelyingParty relyingParty = new OpenIdRelyingParty(null);

        [Authorize, HttpPost]
        public ActionResult Register(Guid id)
        {
            var dinner = dinnerReadModel.GetDinnerById(id);

            if(!dinner.IsUserRegistered(User.Identity.Name))
            {
                var nerd = (NerdIdentity)User.Identity;
                var rsvpForDinnerCommand = new RsvpForDinnerCommand
                                               {DinnerId = id, AttendeeName = nerd.Name, AttendeeId = nerd.UserId};
                commandService.RsvpForDinner(rsvpForDinnerCommand);
            }

            return Content("Thanks - we'll see you there!");
        }

        public ActionResult RsvpBegin(string identifier, Guid id)
        {
            var returnTo = new Uri(new Uri(Realm.AutoDetect), Url.Action("RsvpFinish"));
            var request = relyingParty.CreateRequest(identifier, Realm.AutoDetect, returnTo);
            request.SetUntrustedCallbackArgument("DinnerId", id.ToString());
            request.AddExtension(new ClaimsRequest {Email = DemandLevel.Require, FullName = DemandLevel.Request});
            return request.RedirectingResponse.AsActionResult();
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post), ValidateInput(false)]
        public ActionResult RsvpFinish()
        {
            var response = relyingParty.GetResponse();
            if (response == null)
            {
                return RedirectToAction("Index");
            }

            if (response.Status == AuthenticationStatus.Authenticated)
            {
                var id = Guid.Parse(response.GetUntrustedCallbackArgument("DinnerId"));
                var dinner = dinnerReadModel.GetDinnerById(id);

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
                    var attendeeId = Guid.Parse(response.ClaimedIdentifier);

                    var rsvpForDinnerCommand = new RsvpForDinnerCommand { DinnerId = id, AttendeeName = alias, AttendeeId = attendeeId };
                    commandService.RsvpForDinner(rsvpForDinnerCommand);
                }
            }

            return RedirectToAction("Details", "Dinners", new { id = response.GetUntrustedCallbackArgument("DinnerId") });
        }

        public ActionResult RsvpTwitterBegin(Guid id)
        {
            var callback = new Uri(new Uri(Realm.AutoDetect), Url.Action("RsvpTwitterFinish", new { id = id }));
            return TwitterConsumer.StartSignInWithTwitter(false, callback).AsActionResult();
        }

        public ActionResult RsvpTwitterFinish(Guid id)
        {
            string screenName;
            int userId;
            if (TwitterConsumer.TryFinishSignInWithTwitter(out screenName, out userId))
            {
                var dinner = dinnerReadModel.GetDinnerById(id);

                // NOTE: The alias we've generated for this user isn't guaranteed to be unique.
                string alias = "@" + screenName;
                if (!dinner.IsUserRegistered(alias))
                {
                    var rsvpForDinnerCommand = new RsvpForDinnerCommand { DinnerId = id, AttendeeName = alias };
                    commandService.RsvpForDinner(rsvpForDinnerCommand);
                }
            }

            return RedirectToAction("Details", "Dinners", new { id });
        }
    }
}