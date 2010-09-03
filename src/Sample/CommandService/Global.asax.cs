using System;
using System.Web;
using SimpleCqrs;

namespace Commands
{
    public class Global : HttpApplication
    {
        public static WebRuntime WebRuntime { get; private set; }

        protected void Application_Start(object sender, EventArgs e)
        {
            WebRuntime = new WebRuntime();
            WebRuntime.Start();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {
        }

        protected void Session_End(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }
    }
}