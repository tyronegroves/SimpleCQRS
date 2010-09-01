using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Security.Principal;
using System.Diagnostics;

namespace NerdDinner
{

	public class MvcApplication : System.Web.HttpApplication
	{
		public void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
					"PrettyDetails",
					"{Id}",
						new { controller = "Dinners", action = "Details" },
						new { Id = @"\d+" }
					);


			routes.MapRoute(
					"UpcomingDinners",
					"Dinners/Page/{page}",
					new { controller = "Dinners", action = "Index" }
			);

			routes.MapRoute(
					"Default",                                              // Route name
					"{controller}/{action}/{id}",                           // URL with parameters
					new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
			);

            routes.MapRoute(
                "OpenIdDiscover",
                "Auth/Discover");

		}

        public override void Init()
        {
            this.AuthenticateRequest += new EventHandler(MvcApplication_AuthenticateRequest);
            this.PostAuthenticateRequest += new EventHandler(MvcApplication_PostAuthenticateRequest);
            base.Init();
        }

		void Application_Start()
		{
			RegisterRoutes(RouteTable.Routes);

			ViewEngines.Engines.Clear();
			ViewEngines.Engines.Add(new MobileCapableWebFormViewEngine());
		}

        void MvcApplication_PostAuthenticateRequest(object sender, EventArgs e)
        {
            HttpCookie authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                string encTicket = authCookie.Value;
                if (!String.IsNullOrEmpty(encTicket))
                {
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(encTicket);
                    NerdIdentity id = new NerdIdentity(ticket);
                    GenericPrincipal prin = new GenericPrincipal(id, null);
                    HttpContext.Current.User = prin;
                }
            }
        }

        void MvcApplication_AuthenticateRequest(object sender, EventArgs e)
        {
        }
	}
}