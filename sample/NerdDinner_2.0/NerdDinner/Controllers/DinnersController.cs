using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NerdDinner.Commands;
using NerdDinner.Helpers;
using NerdDinner.Models;
using NerdDinner.Services;

namespace NerdDinner.Controllers
{
    [HandleErrorWithELMAH]
    public class DinnersController : Controller
    {
        private readonly DinnerReadModel dinnerReadModel = new DinnerReadModel();
        private readonly CommandServiceClient commandService = new CommandServiceClient();

        public ActionResult Index(string q, int? page)
        {
            const int pageSize = 25;

            IQueryable<Dinner> dinners;

            //Searching?
            if(!string.IsNullOrWhiteSpace(q))
                dinners = dinnerReadModel.FindDinnersByText(q).OrderBy(d => d.EventDate);
            else
                dinners = dinnerReadModel.FindUpcomingDinners();

            var paginatedDinners = new PaginatedList<Dinner>(dinners, page ?? 0, pageSize);

            return View(paginatedDinners);
        }

        public ActionResult Details(Guid? id)
        {
            if(id == null)
                return new FileNotFoundResult {Message = "No Dinner found due to invalid dinner id"};

            var dinner = dinnerReadModel.GetDinnerById(id.Value);

            if(dinner == null)
                dinner = TempData["Dinner"] as Dinner;

            if(dinner == null)
                return new FileNotFoundResult {Message = "No Dinner found for that id"};

            return View(dinner);
        }

        [Authorize]
        public ActionResult Edit(Guid id)
        {
            var dinner = dinnerReadModel.GetDinnerById(id);

            if(!dinnerReadModel.DinnerIsHostedBy(id, User.Identity.Name))
                return View("InvalidOwner");

            return View(dinner);
        }

        [HttpPost, Authorize]
        public ActionResult Edit(EditDinnerCommand editDinnerCommand)
        {
            var dinnerId = editDinnerCommand.DinnerId;
            var dinner = dinnerReadModel.GetDinnerById(dinnerId);

            if(!dinnerReadModel.DinnerIsHostedBy(dinnerId, User.Identity.Name))
                return View("InvalidOwner");

            try
            {
                var dinnerHost = new DinnerHost();
                var location = new Location();
                UpdateModel(dinnerHost);
                UpdateModel(location);

                editDinnerCommand.Host = dinnerHost;
                editDinnerCommand.Location = location;
                commandService.EditDinner(editDinnerCommand);
                TempData["Dinner"] = dinner;
                return RedirectToAction("Details", new {id = dinner.DinnerId});
            }
            catch
            {
                return View(dinner);
            }
        }

        [Authorize]
        public ActionResult Create()
        {
            return View(new Dinner {EventDate = DateTime.Now.AddDays(7)});
        }

        [HttpPost, Authorize]
        public ActionResult Create(CreateDinnerCommand createDinnerCommand, Dinner dinner)
        {
            if (ModelState.IsValid)
            {
                var nerd = (NerdIdentity)User.Identity;
                var dinnerId = Guid.NewGuid();

                createDinnerCommand.Host = new DinnerHost {HostedById = nerd.UserId, HostedBy = nerd.FriendlyName};
                createDinnerCommand.Location = new Location {Address = dinner.Address, Country = dinner.Country, Latitude = dinner.Latitude, Longitude = dinner.Longitude};
                createDinnerCommand.DinnerId = dinnerId;

                commandService.CreateDinner(createDinnerCommand);

                return RedirectToAction("Details", new { id = dinnerId });
            }

            return View(dinner);
        }

        [Authorize]
        public ActionResult Delete(Guid id)
        {
            var dinner = dinnerReadModel.GetDinnerById(id);

            if (dinner == null)
                return View("NotFound");

            if (!dinnerReadModel.DinnerIsHostedBy(id, User.Identity.Name))
                return View("InvalidOwner");

            return View(dinner);
        }

        [HttpPost, Authorize]
        public ActionResult Delete(Guid id, string confirmButton)
        {
            if (!dinnerReadModel.DinnerExists(id))
                return View("NotFound");

            if (!dinnerReadModel.DinnerIsHostedBy(id, User.Identity.Name))
                return View("InvalidOwner");
            
            commandService.CancelDinner(new CancelDinnerCommand {DinnerId = id});
            
            return View("Deleted");
        }

        public ActionResult Confused()
        {
            return View();
        }

        public ActionResult Trouble()
        {
            return View("Error");
        }

        [Authorize]
        public ActionResult My()
        {
            var nerd = (NerdIdentity)User.Identity;
            var userDinners = dinnerReadModel.FindAllDinnersByUserId(nerd.UserId);
                              
            return View(userDinners);
        }

        public ActionResult WebSlicePopular()
        {
            ViewData["Title"] = "Popular Nerd Dinners";
            var dinners = dinnerReadModel.FindPopularDinners().Take(5);
            return View("WebSlice", dinners);
        }

        public ActionResult WebSliceUpcoming()
        {
            ViewData["Title"] = "Upcoming Nerd Dinners";
            var d = DateTime.Now.AddMonths(2);
            var dinners = dinnerReadModel.FindUpcomingDinners(d);
            return View("WebSlice", dinners);
        }

        protected override void HandleUnknownAction(string actionName)
        {
            throw new HttpException(404, "Action not found");
        }
    }
}