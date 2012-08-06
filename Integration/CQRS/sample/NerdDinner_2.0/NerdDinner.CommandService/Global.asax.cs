using System;
using System.Web;

namespace NerdDinner.CommandService
{
    public class Global : HttpApplication
    {
        public static NerdDinnerCqrsRuntime CqrsRuntime { get; private set; }

        protected void Application_Start(object sender, EventArgs e)
        {
            var nerdDinnerCqrsRuntime = new NerdDinnerCqrsRuntime();
            nerdDinnerCqrsRuntime.Start();
            CqrsRuntime = nerdDinnerCqrsRuntime;
        }

        protected void Application_End(object sender, EventArgs e)
        {
            CqrsRuntime.Shutdown();
        }
    }
}