using System;
using System.Web.Mvc;
using Sample.JesseHouse.Commands;
using Sample.JesseHouse.Models;
using SimpleCqrs.Commanding;

namespace Sample.JesseHouse.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICommandBus commandBus;

        public CustomerController()
        {
            // Again I would use MvcTurbine and have ICommandBus injected into this constructor,
            // but for now just resolve out of the service locator
            commandBus = MvcApplication.Runtime.ServiceLocator.Resolve<ICommandBus>();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(CustomerInputModel customerInputModel)
        {
            // Normal stuff here, create a command and send it on the command bus
            // TODO: Go to the Sample.JesseHouse.Processing project's EndpointConfiguration class next
            var createCustomerCommand = new CreateCustomerCommand();
            createCustomerCommand.CustomerId = Guid.NewGuid();
            createCustomerCommand.FirstName = customerInputModel.FirstName;
            createCustomerCommand.LastName = customerInputModel.LastName;

            commandBus.Send(createCustomerCommand);

            return RedirectToAction("Index");
        }
    }
}