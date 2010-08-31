using System.Web.Mvc;
using Commands;
using Web.Commanding;

namespace Web.Controllers
{
    public class CartController : Controller
    {
        public ActionResult Index()
        {
            var commandService = new CommandServiceClient();
            var result = commandService.CreateCart(new CreateCartCommand());
            return View();
        }
    }
}