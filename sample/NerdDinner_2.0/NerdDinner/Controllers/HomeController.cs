using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NerdDinner.Controllers {

		[HandleErrorWithELMAH]
    public class HomeController : Controller {
    
        public ActionResult Index() {
            return View();
        }

        public ActionResult About() {
            return View();
        }

        public ActionResult PrivacyPolicy()
        {
            return View();
        }
   }
}
