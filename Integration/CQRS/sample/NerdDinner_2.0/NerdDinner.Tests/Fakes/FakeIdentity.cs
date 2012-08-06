using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace NerdDinner.Tests.Fakes
{
    class FakeIdentity
    {
        public static NerdIdentity CreateIdentity(string userName)
        {
            FormsAuthenticationTicket authTicket = new
                FormsAuthenticationTicket(1, //version
                userName, // user name
                DateTime.Now,             //creation
                DateTime.Now.AddMinutes(30), //Expiration
                false, //Persistent
                userName);

            var nerdIdentity = new NerdIdentity(authTicket);
            return nerdIdentity;
        }
    }
}
