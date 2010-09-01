using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NerdDinner.Controllers;
using NerdDinner.Helpers;
using NerdDinner.Models;
using NerdDinner.Tests.Fakes;
using System.Web.Routing;
using System.Web.Security;

namespace NerdDinner.Tests.Controllers {
 
    [TestClass]
    public class DinnersControllerTest {
		private const int NumberOfCountries = 256;

        DinnersController CreateDinnersController() {
            var testData = FakeDinnerData.CreateTestDinners();
            var repository = new FakeDinnerRepository(testData);

            return new DinnersController(repository);
        }

        DinnersController CreateDinnersControllerAs(string userName) {

            var mock = new Mock<ControllerContext>();
            mock.SetupGet(p => p.HttpContext.User.Identity.Name).Returns(userName);
            mock.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);

            var controller = CreateDinnersController();
            controller.ControllerContext = mock.Object;

            return controller;
        }


        [TestMethod]
        public void DetailsAction_Should_Return_View_For_Dinner() {

            // Arrange
            var controller = CreateDinnersController();

            // Act
            var result = controller.Details(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void DetailsAction_Should_Return_NotFoundView_For_BogusDinner() {

            // Arrange
            var controller = CreateDinnersController();

            // Act
			var result = controller.Details(999) as ActionResult;

            // Assert
			Assert.IsInstanceOfType(result, typeof(FileNotFoundResult));
			Assert.AreEqual("No Dinner found for that id", ((FileNotFoundResult)result).Message);
        }

        [TestMethod]
        public void EditAction_Should_Return_View_For_ValidDinner() {

            // Arrange
            var controller = CreateDinnersControllerAs("SomeUser");

            // Act
            var result = controller.Edit(1) as ViewResult;

            // Assert
            Assert.IsInstanceOfType(result.ViewData.Model, typeof(Dinner));
        }

        [TestMethod]
        public void EditAction_Should_Return_View_For_InValidOwner() {

            // Arrange
            var controller = CreateDinnersControllerAs("SomeOtherUser");

            // Act
            var result = controller.Edit(1) as ViewResult;

            // Assert
            Assert.AreEqual(result.ViewName, "InvalidOwner");
        }

        [TestMethod]
        public void EditAction_Should_Redirect_When_Update_Successful() {

            // Arrange
            var controller = CreateDinnersControllerAs("SomeUser");
            int id = 1;

            FormCollection formValues = new FormCollection() {
                { "Dinner.Title", "Another value" },
                { "Dinner.Description", "Another description" }
            };

            controller.ValueProvider = formValues.ToValueProvider();
            
            // Act
            var result = controller.Edit(id, formValues) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Details", result.RouteValues["Action"]);
            Assert.AreEqual(id, result.RouteValues["id"]);
        }

        [TestMethod]
        public void EditAction_Should_Redisplay_With_Errors_When_Update_Fails() {

            // Arrange
            var controller = CreateDinnersControllerAs("SomeUser");
            int id = 1;

            FormCollection formValues = new FormCollection() {
                { "Dinner.EventDate", "Bogus date value!!!"}
            };

            controller.ValueProvider = formValues.ToValueProvider();

            // Act
            var result = controller.Edit(id, formValues) as ViewResult;

            // Assert
            Assert.IsNotNull(result, "Expected redisplay of view");
            Assert.IsTrue(result.ViewData.ModelState.Sum(p => p.Value.Errors.Count) > 0, "Expected Errors");
        }

        [TestMethod]
        public void IndexAction_Should_Return_View() {

            // Arrange
            var controller = CreateDinnersControllerAs("robcon");

            // Act
            var result = controller.Index(string.Empty, 0);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void IndexAction_Returns_TypedView_Of_List_Dinner() {
            // Arrange
            var controller = CreateDinnersControllerAs("robcon");

            // Act
            ViewResult result = (ViewResult)controller.Index(null, 0);

            // Assert
            Assert.IsInstanceOfType(result.ViewData.Model, typeof(IList<Dinner>), "Index does not have an IList<Dinner> as a ViewModel");
        }


        [TestMethod]
        public void IndexAction_Should_Return_PagedList() {
            
            // Arrange
            var controller = CreateDinnersControllerAs("robcon");

            // Act
            //Get first page
            ViewResult result = (ViewResult)controller.Index(null, 0);
            
            // Assert
            Assert.IsInstanceOfType(result.ViewData.Model, typeof(PaginatedList<Dinner>));
        }


        [TestMethod]
        public void IndexAction_Should_Return_PagedList_With_Total_of_101_And_Total_10_Pages() {

            // Arrange
            var controller = CreateDinnersControllerAs("robcon");

            // Act
            // Get first page
            ViewResult result = (ViewResult)controller.Index(null, 0);
            PaginatedList<Dinner> list = result.ViewData.Model as PaginatedList<Dinner>;

            // Assert
            Assert.AreEqual(101, list.TotalCount);
            Assert.AreEqual(5, list.TotalPages);
        }

        [TestMethod]
        public void IndexAction_Should_Return_PagedList_With_Total_of_101_And_Total_5_Pages_Given_Null()
        {

            // Arrange
            var controller = CreateDinnersControllerAs("robcon");

            // Act
            // Get first page
            ViewResult result = (ViewResult)controller.Index(null, null);
            PaginatedList<Dinner> list = result.ViewData.Model as PaginatedList<Dinner>;

            // Assert
            Assert.AreEqual(101, list.TotalCount);
            Assert.AreEqual(5, list.TotalPages);
        }

		[TestMethod]
		public void IndexAction_With_Dinner_Just_Started_Should_Show_Dinner()
		{
			// Arrange 
			var testData = FakeDinnerData.CreateTestDinners();
			var dinner = FakeDinnerData.CreateDinner();
			dinner.EventDate = DateTime.Now.AddHours(-1);
			dinner.Title = "Dinner which just started";
			testData.Add(dinner);
			var repository = new FakeDinnerRepository(testData);

			var controller = new DinnersController(repository);

			// Act
			// Get first page
			ViewResult result = (ViewResult)controller.Index(null, null);
			PaginatedList<Dinner> list = result.ViewData.Model as PaginatedList<Dinner>;

			// Assert
			Assert.AreEqual("Dinner which just started", list.First().Title);
		}

        [TestMethod]
        public void IndexAction_With_Search_Term_Should_Filter()
        {
            // Arrange 
            string searchterm = "Dinner we will be searching for (spaghetti)";

            var testData = FakeDinnerData.CreateTestDinners();
            var dinner = FakeDinnerData.CreateDinner();
            dinner.Title = searchterm;
            testData.Add(dinner);
            var repository = new FakeDinnerRepository(testData);

            var controller = new DinnersController(repository);

            // Act
            // Get first page
            ViewResult result = (ViewResult)controller.Index("etti", null);
            PaginatedList<Dinner> list = result.ViewData.Model as PaginatedList<Dinner>;

            // Assert
            Assert.AreEqual(searchterm, list.First().Title);
        }

		[TestMethod]
        public void DetailsAction_Should_Return_ViewResult() {

            // Arrange
            var controller = CreateDinnersControllerAs("scottgu");

            // Act
            var result = controller.Details(1);

            // Assert
            Assert.IsNotNull(result, "There is no Details action");
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        
        //[TestMethod]
        //public void DownloadCalendarAction_Should_Return_ContentResult() {
            
        //    // Arrange
        //    var mockResponse = new Mock<System.Web.HttpResponseBase>();
        //    var mockContext = new Mock<System.Web.HttpContextBase>();
        //    mockContext.Setup(p => p.Response).Returns(mockResponse.Object);
        //    var requestContext = new RequestContext(mockContext.Object, new RouteData());

        //    var controller = CreateDinnersController();
        //    controller.ControllerContext = new ControllerContext(requestContext, controller);

        //    // Act
        //    var result = controller.DownloadCalendar(1);

        //    // Assert
        //    Assert.IsNotNull(result, "There is no DownloadCalendar action");
        //    Assert.IsInstanceOfType(result, typeof(ContentResult));
        //}

        [TestMethod]
        public void DetailsAction_Should_Return_FileNotFoundResult_For_NullDinnerId() {
            // Arrange
            var controller = CreateDinnersControllerAs("scottgu");

            // Act
            FileNotFoundResult result = (FileNotFoundResult)controller.Details(null);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void DetailsAction_Should_Return_FileNotFoundResult_For_Dinner_999() {

            // Arrange
            var controller = CreateDinnersControllerAs("scottgu");

            // Act
            FileNotFoundResult result = (FileNotFoundResult)controller.Details(999);

            // Assert
            Assert.IsNotNull(result);
        }


        [TestMethod]
        public void DetailsAction_Should_Have_ViewModel_Is_Dinner() {

            // Arrange
            var controller = CreateDinnersControllerAs("scottgu");

            // Act
            ViewResult result = (ViewResult)controller.Details(1);

            // Assert
            Assert.IsInstanceOfType(result.ViewData.Model, typeof(Dinner));

        }

        [TestMethod]
        public void DetailsAction_Should_Return_Dinner_HostedBy_SomeUser() {

            // Arrange
            var controller = CreateDinnersControllerAs("SomeUser");

            // Act

            //the mocked user is "SomeUser", who also owns the dinner
            ViewResult result = (ViewResult)controller.Details(1);
            Dinner model = result.ViewData.Model as Dinner;
            
            // Assert
            
            //scottgu, our mock user, is the host in the fake
            Assert.IsTrue(model.IsHostedBy("SomeUser"));
        }

        [TestMethod]
        public void CreateAction_Should_Return_ViewResult() {

            // Arrange
            var controller = CreateDinnersControllerAs("scottgu");

            // Act
            var result = controller.Create();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void CreateAction_Should_Return_DinnerFormViewModel() {
            
            // Arrange
            var controller = CreateDinnersControllerAs("scottgu");
            
            // Act
            ViewResult result = (ViewResult)controller.Create();

            // Assert
            Assert.IsInstanceOfType(result.ViewData.Model, typeof(DinnerFormViewModel));
        }

        [TestMethod]
        public void CreateAction_Should_Return_DinnerFormViewModel_With_New_Dinner_And_Countries_List() {

            // Arrange
            var controller = CreateDinnersControllerAs("scottgu");

            // Act
            ViewResult result = (ViewResult)controller.Create();
            DinnerFormViewModel model = (DinnerFormViewModel)result.ViewData.Model;
            
            // Assert
            Assert.IsNotNull(model.Dinner);
            Assert.AreEqual(NumberOfCountries, model.Countries.Count());
        }

        [TestMethod]
        public void CreateAction_Should_Return_DinnerFormViewModel_With_New_Dinner_7_Days_In_Future() {
            
            // Arrange
            var controller = CreateDinnersControllerAs("scottgu");
            
            // Act
            ViewResult result = (ViewResult)controller.Create();
            
            // Assert
            DinnerFormViewModel model = (DinnerFormViewModel)result.ViewData.Model;
            Assert.IsTrue(model.Dinner.EventDate > DateTime.Today.AddDays(6) && model.Dinner.EventDate < DateTime.Today.AddDays(8));
        }

        [TestMethod]
        public void CreateAction_With_New_Dinner_Should_Return_View_And_Repo_Should_Contain_New_Dinner()
        {
            // Arrange 
            var mock = new Mock<ControllerContext>();

            var nerdIdentity = FakeIdentity.CreateIdentity("SomeUser");
            var testData = FakeDinnerData.CreateTestDinners();
            var repository = new FakeDinnerRepository(testData);
            var controller = new DinnersController(repository);
            controller.ControllerContext = mock.Object;
            mock.SetupGet(p => p.HttpContext.User.Identity).Returns(nerdIdentity);

            var dinner = FakeDinnerData.CreateDinner();

            // Act
            ActionResult result = (ActionResult)controller.Create(dinner);

            // Assert
            Assert.AreEqual(102, repository.FindAllDinners().Count());
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public void EditAction_Should_Return_ViewResult() {
            
            // Arrange
            var controller = CreateDinnersControllerAs("robcon");

            // Act
            var result = controller.Edit(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void EditAction_Returns_InvalidOwner_View_When_Not_SomeUser() {
            
            // Arrange
            var controller = CreateDinnersControllerAs("robcon");

            // Act
            ViewResult result = controller.Edit(1) as ViewResult;

            // Assert
            Assert.AreEqual("InvalidOwner", result.ViewName);
        }

        [TestMethod]
        public void EditAction_Uses_DinnerFormViewModel() {

            // Arrange
            var controller = CreateDinnersControllerAs("SomeUser");

            // Act
            ViewResult result = controller.Edit(1) as ViewResult;

            // Assert
            Assert.IsInstanceOfType(result.ViewData.Model, typeof(Dinner));
        }

        [TestMethod]
        public void EditAction_Retrieves_Dinner_1_From_Repo() {

            // Arrange
            var controller = CreateDinnersControllerAs("SomeUser");

            // Act
            ViewResult result = controller.Edit(1) as ViewResult;

            // Assert
            var model = result.ViewData.Model as Dinner;
            Assert.AreEqual(1, model.DinnerID);
        }

        [TestMethod]
        public void EditAction_Saves_Changes_To_Dinner_1()
        {
            // Arrange
            var repo = new FakeDinnerRepository(FakeDinnerData.CreateTestDinners());
            var controller = CreateDinnersControllerAs("SomeUser");
            var form = FakeDinnerData.CreateDinnerFormCollection();
            form["Dinner.Description"] = "New, Updated Description";
            controller.ValueProvider = form.ToValueProvider();

            // Act
            ActionResult result = (ActionResult)controller.Edit(1, form);
            ViewResult detailResult = (ViewResult)controller.Details(1);
            var dinner = detailResult.ViewData.Model as Dinner;

            // Assert
            Assert.AreEqual(5, controller.ModelState.Count);
            Assert.AreEqual("New, Updated Description", dinner.Description);
        }

        [TestMethod]
        public void EditAction_Fails_With_Wrong_Owner() {
            
            // Arrange
            var repo = new FakeDinnerRepository(FakeDinnerData.CreateTestDinners());
            var controller = CreateDinnersControllerAs("fred");
            var form = FakeDinnerData.CreateDinnerFormCollection();
            controller.ValueProvider = form.ToValueProvider();

            // Act
            ViewResult result = (ViewResult)controller.Edit(1, form);

            // Assert
            Assert.AreEqual("InvalidOwner", result.ViewName);
        }

		//Unit test is invalid until phone number verification is turned back on
		//[TestMethod]
		//public void DinnersController_Edit_Post_Should_Fail_Given_Bad_US_Phone_Number() {
            
		//    // Arrange
		//    var controller = CreateDinnersControllerAs("someuser");
		//    var form = FakeDinnerData.CreateDinnerFormCollection();
		//    form["ContactPhone"] = "foo"; //BAD
		//    controller.ValueProvider = form.ToValueProvider();

		//    // Act
		//    var result = controller.Edit(1, form);

		//    // Assert
		//    Assert.IsInstanceOfType(result, typeof(ViewResult));
		//    var viewResult = (ViewResult)result;
		//    Assert.IsFalse(viewResult.ViewData.ModelState.IsValid);
		//    Assert.AreEqual(1, viewResult.ViewData.ModelState.Sum(p => p.Value.Errors.Count), "Expected Errors");
		//    ModelState m = viewResult.ViewData.ModelState["ContactPhone"];
		//    Assert.IsTrue(m.Errors.Count == 1);
		//}


        [TestMethod]
        public void DeleteAction_Should_Return_View()
        {
            // Arrange
            var controller = CreateDinnersControllerAs("someuser");

            // Act
            var result = controller.Delete(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void DeleteAction_Should_Return_NotFound_For_999()
        {

            // Arrange
            var controller = CreateDinnersControllerAs("scottgu");

            // Act
            ViewResult result = controller.Delete(999) as ViewResult;

            // Assert
            Assert.AreEqual("NotFound", result.ViewName);
        }

        [TestMethod]
        public void DeleteAction_Should_Return_InvalidOwner_For_Robcon()
        {

            // Arrange
            var controller = CreateDinnersControllerAs("robcon");

            // Act
            ViewResult result = controller.Delete(1) as ViewResult;

            // Assert
            Assert.AreEqual("InvalidOwner", result.ViewName);
        }

        [TestMethod]
        public void DeleteAction_Should_Delete_Dinner_1_And_Returns_Deleted_View() {

            // Arrange
            var controller = CreateDinnersControllerAs("SomeUser");

            // Act
            ViewResult result = controller.Delete(1) as ViewResult;

            // Assert
            Assert.AreNotEqual("NotFound", result.ViewName);
            Assert.AreNotEqual("InvalidOwner", result.ViewName);
        }

        [TestMethod]
        public void DeleteAction_With_Confirm_Should_Delete_Dinner_1_And_Returns_Deleted_View()
        {

            // Arrange
            var controller = CreateDinnersControllerAs("SomeUser");

            // Act
            ViewResult result = controller.Delete(1, String.Empty) as ViewResult;

            // Assert
            Assert.AreNotEqual("NotFound", result.ViewName);
            Assert.AreNotEqual("InvalidOwner", result.ViewName);
        }

        
        [TestMethod]
        public void DeleteAction_Should_Fail_With_NotFound_Given_Invalid_Dinner()
        {

            // Arrange
            var controller = CreateDinnersControllerAs("robcon");

            // Act
            ViewResult result = controller.Delete(200, "") as ViewResult;

            // Assert
            Assert.AreEqual("NotFound", result.ViewName);
        }

        [TestMethod]
        public void DeleteAction_Should_Fail_With_InvalidOwner_Given_Wrong_User()
        {

            // Arrange
            var controller = CreateDinnersControllerAs("scottha");

            // Act
            ViewResult result = controller.Delete(1, "") as ViewResult;

            // Assert
            Assert.AreEqual("InvalidOwner", result.ViewName);
        }
    }
}
