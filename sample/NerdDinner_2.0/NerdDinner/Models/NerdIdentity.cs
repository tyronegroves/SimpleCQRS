using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;

namespace NerdDinner
{
    public class NerdIdentity : IIdentity
    {
        private System.Web.Security.FormsAuthenticationTicket ticket;
        private readonly string friendlyName;
        private readonly Guid userId;

        public NerdIdentity(System.Web.Security.FormsAuthenticationTicket ticket)
        {
            this.ticket = ticket;
            var userData = ticket.UserData.Split('|');
            
            if(userData.Length != 2) return;
            friendlyName = userData[0];
            userId = new Guid(userData[1]);
        }

        public string AuthenticationType
        {
            get { return "Nerd"; }
        }

        public bool IsAuthenticated
        {
            get { return true; }
        }

        public string Name
        {
            get { return ticket.Name; }
        }

        public string FriendlyName
        {
            get { return friendlyName; }
        }

        public Guid UserId
        {
            get { return userId; }
        }
    }
}
