//-----------------------------------------------------------------------
// <copyright file="AuthController.cs" company="Andrew Arnott">
//     Copyright (c) Andrew Arnott. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace MvcRelyingParty.Controllers {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Web;
	using System.Web.Mvc;
	using DotNetOpenAuth.Messaging;
	using DotNetOpenAuth.OpenId;
	using DotNetOpenAuth.OpenId.RelyingParty;
    using System.Web.Security;
    using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;

	public class AuthController : Controller {

		/// <summary>
		/// Gets the OpenID relying party to use for logging users in.
		/// </summary>
        public IOpenIdRelyingParty RelyingParty = new OpenIdRelyingPartyService();

		private Uri PrivacyPolicyUrl {
			get {
				return Url.ActionFull("PrivacyPolicy", "Home");
			}
		}

		/// <summary>
		/// Performs discovery on a given identifier.
		/// </summary>
		/// <param name="identifier">The identifier on which to perform discovery.</param>
		/// <returns>The JSON result of discovery.</returns>
		public ActionResult Discover(string identifier) {
			if (!this.Request.IsAjaxRequest()) {
				throw new InvalidOperationException();
			}

			return RelyingParty.AjaxDiscovery(
				identifier,
				Realm.AutoDetect,
				Url.ActionFull("PopUpReturnTo"),
				this.PrivacyPolicyUrl);
		}

		/// <summary>
		/// Prepares a web page to help the user supply his login information.
		/// </summary>
		/// <returns>The action result.</returns>
		public ActionResult LogOn() {
			return View();
		}

		/// <summary>
		/// Prepares a web page to help the user supply his login information.
		/// </summary>
		/// <returns>The action result.</returns>
		public ActionResult LogOnPopUp() {
			return View();
		}

		/// <summary>
		/// Handles the positive assertion that comes from Providers to Javascript running in the browser.
		/// </summary>
		/// <returns>The action result.</returns>
		/// <remarks>
		/// This method instructs ASP.NET MVC to <i>not</i> validate input
		/// because some OpenID positive assertions messages otherwise look like
		/// hack attempts and result in errors when validation is turned on.
		/// </remarks>
		[AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post), ValidateInput(false)]
		public ActionResult PopUpReturnTo() {
			return RelyingParty.ProcessAjaxOpenIdResponse();
		}

		/// <summary>
		/// Handles the positive assertion that comes from Providers.
		/// </summary>
		/// <param name="openid_openidAuthData">The positive assertion obtained via AJAX.</param>
		/// <returns>The action result.</returns>
		/// <remarks>
		/// This method instructs ASP.NET MVC to <i>not</i> validate input
		/// because some OpenID positive assertions messages otherwise look like
		/// hack attempts and result in errors when validation is turned on.
		/// </remarks>
		[HttpPost, ValidateInput(false)]
		public ActionResult LogOnPostAssertion(string openid_openidAuthData) {
			IAuthenticationResponse response;
			if (!string.IsNullOrEmpty(openid_openidAuthData)) {
				var auth = new Uri(openid_openidAuthData);
				var headers = new WebHeaderCollection();
				foreach (string header in Request.Headers) {
					headers[header] = Request.Headers[header];
				}

				// Always say it's a GET since the payload is all in the URL, even the large ones.
				HttpRequestInfo clientResponseInfo = new HttpRequestInfo("GET", auth, auth.PathAndQuery, headers, null);
				response = RelyingParty.GetResponse(clientResponseInfo);
			} else {
				response = RelyingParty.GetResponse();
			}
			if (response != null) {
				switch (response.Status) {
					case AuthenticationStatus.Authenticated:
                        string alias = response.FriendlyIdentifierForDisplay;
                        var sreg = response.GetExtension<ClaimsResponse>();
                        if (sreg != null && sreg.MailAddress != null) {
                            alias = sreg.MailAddress.User;
                        }
                        if (sreg != null && !string.IsNullOrEmpty(sreg.FullName)) {
                            alias = sreg.FullName;
                        }

                        FormsAuthenticationTicket authTicket = new
                            FormsAuthenticationTicket(1, //version
                            response.ClaimedIdentifier, // user name
                            DateTime.Now,             //creation
                            DateTime.Now.AddMinutes(30), //Expiration
                            false, //Persistent
                            alias); 					
                           
                        string encTicket = FormsAuthentication.Encrypt(authTicket);

                        this.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));


                        string returnUrl = Request.Form["returnUrl"];
						if (!String.IsNullOrEmpty(returnUrl)) {
							return Redirect(returnUrl);
						} else {
							return RedirectToAction("Index", "Home");
						}
					case AuthenticationStatus.Canceled:
						ModelState.AddModelError("OpenID", "It looks like you canceled login at your OpenID Provider.");
						break;
					case AuthenticationStatus.Failed:
						ModelState.AddModelError("OpenID", response.Exception.Message);
						break;
				}
			}

			// If we're to this point, login didn't complete successfully.
			// Show the LogOn view again to show the user any errors and
			// give another chance to complete login.
			return View("LogOn");
		}
	}
}
