using System;
using System.Web;
using SimpleCqrs;

namespace Commands
{
    public class Global : HttpApplication
    {
        public static IServiceLocator ServiceLocator { get; private set; }

        protected void Application_Start(object sender, EventArgs e)
        {
            var bootstrapper = new WebBootstrapper();
            ServiceLocator = bootstrapper.Run();
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