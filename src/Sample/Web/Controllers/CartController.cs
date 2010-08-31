using System;
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
            var createCartCommand = new CreateCartCommand();
            Session["CartId"] = createCartCommand.Id;
            commandService.CreateCart(createCartCommand);
            return View();
        }

        public ActionResult AddProductToCart()
        {
            return View(new AddProductToCartCommand {Id = (Guid)Session["CartId"]});
        }

        [HttpPost]
        public ActionResult AddProductToCart(AddProductToCartCommand addProductToCartCommand)
        {
            var commandService = new CommandServiceClient();
            commandService.AddProductToCart(addProductToCartCommand);
            return View();
        }
    }
}