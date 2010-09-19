using System;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NerdDinner.Commands;
using NerdDinner.Controllers;
using NerdDinner.Models;
using NerdDinner.Services;

namespace NerdDinner.Tests.Controllers
{
    [TestClass]
    public class AccountControllerTest
    {
        [TestMethod]
        public void ChangePasswordGetReturnsView()
        {
            // Arrange
            var controller = GetAccountController();

            // Act
            var result = (ViewResult) controller.ChangePassword();

            // Assert
            Assert.AreEqual(6, result.ViewData["PasswordLength"]);
        }

        [TestMethod]
        public void ChangePasswordPostRedirectsOnSuccess()
        {
            // Arrange
            var controller = GetAccountController();

            // Act
            var result = (RedirectToRouteResult) controller.ChangePassword("oldPass", "newPass", "newPass");

            // Assert
            Assert.AreEqual("ChangePasswordSuccess", result.RouteValues["action"]);
        }

        [TestMethod]
        public void ChangePasswordPostReturnsViewIfCurrentPasswordNotSpecified()
        {
            // Arrange
            var controller = GetAccountController();

            // Act
            var result = (ViewResult) controller.ChangePassword("", "newPassword", "newPassword");

            // Assert
            Assert.AreEqual(6, result.ViewData["PasswordLength"]);
            Assert.AreEqual("You must specify a current password.",
                            result.ViewData.ModelState["currentPassword"].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void ChangePasswordPostReturnsViewIfNewPasswordDoesNotMatchConfirmPassword()
        {
            // Arrange
            var controller = GetAccountController();

            // Act
            var result = (ViewResult) controller.ChangePassword("currentPassword", "newPassword", "otherPassword");

            // Assert
            Assert.AreEqual(6, result.ViewData["PasswordLength"]);
            Assert.AreEqual("The new password and confirmation password do not match.",
                            result.ViewData.ModelState["_FORM"].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void ChangePasswordPostReturnsViewIfNewPasswordIsNull()
        {
            // Arrange
            var controller = GetAccountController();

            // Act
            var result = (ViewResult) controller.ChangePassword("currentPassword", null, null);

            // Assert
            Assert.AreEqual(6, result.ViewData["PasswordLength"]);
            Assert.AreEqual("You must specify a new password of 6 or more characters.",
                            result.ViewData.ModelState["newPassword"].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void ChangePasswordPostReturnsViewIfNewPasswordIsTooShort()
        {
            // Arrange
            var controller = GetAccountController();

            // Act
            var result = (ViewResult) controller.ChangePassword("currentPassword", "12345", "12345");

            // Assert
            Assert.AreEqual(6, result.ViewData["PasswordLength"]);
            Assert.AreEqual("You must specify a new password of 6 or more characters.",
                            result.ViewData.ModelState["newPassword"].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void ChangePasswordPostReturnsViewIfProviderRejectsPassword()
        {
            // Arrange
            var controller = GetAccountController();

            // Act
            var result = (ViewResult) controller.ChangePassword("oldPass", "badPass", "badPass");

            // Assert
            Assert.AreEqual(6, result.ViewData["PasswordLength"]);
            Assert.AreEqual("The current password is incorrect or the new password is invalid.",
                            result.ViewData.ModelState["_FORM"].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void ChangePasswordSuccess()
        {
            // Arrange
            var controller = GetAccountController();

            // Act
            var result = (ViewResult) controller.ChangePasswordSuccess();

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ConstructorSetsProperties()
        {
            // Arrange
            IFormsAuthentication formsAuth = new MockFormsAuthenticationService();
            var membershipReadModel = new MembershipReadModel();

            // Act  
            var controller = new AccountController(formsAuth, new CommandServiceClient(), membershipReadModel);

            // Assert
            Assert.AreEqual(formsAuth, controller.FormsAuth, "FormsAuth property did not match.");
            Assert.AreEqual(membershipReadModel, controller.MembershipReadModel,
                            "MembershipReadModel property did not match.");
        }

        [TestMethod]
        public void ConstructorSetsPropertiesToDefaultValues()
        {
            // Act
            var controller = new AccountController();

            // Assert
            Assert.IsNotNull(controller.FormsAuth, "FormsAuth property is null.");
            Assert.IsNotNull(controller.MembershipReadModel, "MembershipReadModel property is null.");
        }

        [TestMethod]
        public void LoginGet()
        {
            // Arrange
            var controller = GetAccountController();

            // Act
            var result = (ViewResult) controller.LogOn();

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void LoginPostRedirectsHomeIfLoginSuccessfulButNoReturnUrlGiven()
        {
            // Arrange
            var controller = GetAccountController();

            // Act
            var result = (RedirectToRouteResult) controller.LogOn("someUser", "goodPass", true, null);

            // Assert
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod]
        public void LoginPostRedirectsToReturnUrlIfLoginSuccessfulAndReturnUrlGiven()
        {
            // Arrange
            var controller = GetAccountController();

            // Act
            var result = (RedirectResult) controller.LogOn("someUser", "goodPass", false, "someUrl");

            // Assert
            Assert.AreEqual("someUrl", result.Url);
        }

        [TestMethod]
        public void LoginPostReturnsViewIfPasswordNotSpecified()
        {
            // Arrange
            var controller = GetAccountController();

            // Act
            var result = (ViewResult) controller.LogOn("username", "", true, null);

            // Assert
            Assert.AreEqual(true, result.ViewData["rememberMe"]);
            Assert.AreEqual("You must specify a password.",
                            result.ViewData.ModelState["password"].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void LoginPostReturnsViewIfUsernameNotSpecified()
        {
            // Arrange
            var controller = GetAccountController();

            // Act
            var result = (ViewResult) controller.LogOn("", "somePass", false, null);

            // Assert
            Assert.AreEqual(false, result.ViewData["rememberMe"]);
            Assert.AreEqual("You must specify a username.",
                            result.ViewData.ModelState["username"].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void LoginPostReturnsViewIfUsernameOrPasswordIsIncorrect()
        {
            // Arrange
            var controller = GetAccountController();

            // Act
            var result = (ViewResult) controller.LogOn("someUser", "badPass", true, null);

            // Assert
            Assert.AreEqual(true, result.ViewData["rememberMe"]);
            Assert.AreEqual("The username or password provided is incorrect.",
                            result.ViewData.ModelState["_FORM"].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void LogOff()
        {
            // Arrange
            var controller = GetAccountController();

            // Act
            var result = (RedirectToRouteResult) controller.LogOff();

            // Assert
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod]
        public void RegisterGet()
        {
            // Arrange
            var controller = GetAccountController();

            // Act
            var result = (ViewResult) controller.Register();

            // Assert
            Assert.AreEqual(6, result.ViewData["PasswordLength"]);
        }

        [TestMethod]
        public void RegisterPostRedirectsHomeIfRegistrationSuccessful()
        {
            // Arrange
            var controller = GetAccountController();
            var registerUserCommand = new RegisterUserCommand
                                          {UserName = "someUser", Email = "email", Password = "goodPass"};

            // Act
            var result = (RedirectToRouteResult) controller.Register(registerUserCommand, "goodPass");

            // Assert
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod]
        public void RegisterPostReturnsViewIfEmailNotSpecified()
        {
            // Arrange
            var controller = GetAccountController();
            var registerUserCommand = new RegisterUserCommand {UserName = "username", Email = "", Password = "password"};

            // Act
            var result = (ViewResult) controller.Register(registerUserCommand, "password");

            // Assert
            Assert.AreEqual(6, result.ViewData["PasswordLength"]);
            Assert.AreEqual("You must specify an email address.",
                            result.ViewData.ModelState["email"].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void RegisterPostReturnsViewIfNewPasswordDoesNotMatchConfirmPassword()
        {
            // Arrange
            var controller = GetAccountController();
            var registerUserCommand = new RegisterUserCommand
                                          {UserName = "username", Email = "email", Password = "password"};

            // Act
            var result = (ViewResult) controller.Register(registerUserCommand, "password2");

            // Assert
            Assert.AreEqual(6, result.ViewData["PasswordLength"]);
            Assert.AreEqual("The new password and confirmation password do not match.",
                            result.ViewData.ModelState["_FORM"].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void RegisterPostReturnsViewIfPasswordIsNull()
        {
            // Arrange
            var controller = GetAccountController();
            var registerUserCommand = new RegisterUserCommand {UserName = "username", Email = "email", Password = null};


            // Act
            var result = (ViewResult) controller.Register(registerUserCommand, null);

            // Assert
            Assert.AreEqual(6, result.ViewData["PasswordLength"]);
            Assert.AreEqual("You must specify a password of 6 or more characters.",
                            result.ViewData.ModelState["password"].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void RegisterPostReturnsViewIfPasswordIsTooShort()
        {
            // Arrange
            var controller = GetAccountController();
            var registerUserCommand = new RegisterUserCommand
                                          {
                                              UserName = "username",
                                              Email = "email",
                                              Password = "12345"
                                          };

            // Act
            var result = (ViewResult) controller.Register(registerUserCommand, "12345");

            // Assert
            Assert.AreEqual(6, result.ViewData["PasswordLength"]);
            Assert.AreEqual("You must specify a password of 6 or more characters.",
                            result.ViewData.ModelState["password"].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void RegisterPostReturnsViewIfRegistrationFails()
        {
            // Arrange
            var controller = GetAccountController();
            var registerUserCommand = new RegisterUserCommand
                                          {
                                              UserName = "someUser",
                                              Email = "DuplicateUserName",
                                              Password = "badPass"
                                          };

            // Act
            var result = (ViewResult) controller.Register(registerUserCommand, "badPass");

            // Assert
            Assert.AreEqual(6, result.ViewData["PasswordLength"]);
            Assert.AreEqual("Username already exists. Please enter a different user name.",
                            result.ViewData.ModelState["_FORM"].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void RegisterPostReturnsViewIfUsernameNotSpecified()
        {
            // Arrange
            var controller = GetAccountController();
            var registerUserCommand = new RegisterUserCommand
                                          {
                                              UserName = "",
                                              Email = "email",
                                              Password = "password"
                                          };

            // Act
            var result = (ViewResult)controller.Register(registerUserCommand, "password");

            // Assert
            Assert.AreEqual(6, result.ViewData["PasswordLength"]);
            Assert.AreEqual("You must specify a username.",
                            result.ViewData.ModelState["username"].Errors[0].ErrorMessage);
        }

        private static AccountController GetAccountController()
        {
            IFormsAuthentication formsAuth = new MockFormsAuthenticationService();
            MembershipProvider membershipProvider = new MockMembershipProvider();
            var membershipService = new AccountMembershipService(membershipProvider);
            var controller = new AccountController(formsAuth, new CommandServiceClient(), new MembershipReadModel());

            var controllerContext = new ControllerContext(new MockHttpContext(), new RouteData(), controller);
            controller.ControllerContext = controllerContext;
            return controller;
        }

        public class MockFormsAuthenticationService : IFormsAuthentication
        {
            public void SignIn(string userName, bool createPersistentCookie)
            {
            }

            public void SignOut()
            {
            }
        }

        public class MockIdentity : IIdentity
        {
            public string AuthenticationType
            {
                get { return "MockAuthentication"; }
            }

            public bool IsAuthenticated
            {
                get { return true; }
            }

            public string Name
            {
                get { return "someUser"; }
            }
        }

        public class MockPrincipal : IPrincipal
        {
            private IIdentity _identity;

            public IIdentity Identity
            {
                get
                {
                    if (_identity == null)
                    {
                        _identity = new MockIdentity();
                    }
                    return _identity;
                }
            }

            public bool IsInRole(string role)
            {
                return false;
            }
        }

        public class MockMembershipUser : MembershipUser
        {
            public override bool ChangePassword(string oldPassword, string newPassword)
            {
                return newPassword.Equals("newPass");
            }
        }

        public class MockHttpContext : HttpContextBase
        {
            private IPrincipal _user;

            public override IPrincipal User
            {
                get
                {
                    if (_user == null)
                    {
                        _user = new MockPrincipal();
                    }
                    return _user;
                }
                set { _user = value; }
            }

            public override HttpResponseBase Response
            {
                get { return new MockHttpResponse(); }
            }
        }

        public class MockHttpResponse : HttpResponseBase
        {
            public override HttpCookieCollection Cookies
            {
                get { return new HttpCookieCollection(); }
            }
        }


        public class MockMembershipProvider : MembershipProvider
        {
            public override string ApplicationName { get; set; }

            public override bool EnablePasswordReset
            {
                get { return false; }
            }

            public override bool EnablePasswordRetrieval
            {
                get { return false; }
            }

            public override int MaxInvalidPasswordAttempts
            {
                get { return 0; }
            }

            public override int MinRequiredNonAlphanumericCharacters
            {
                get { return 0; }
            }

            public override int MinRequiredPasswordLength
            {
                get { return 6; }
            }

            public override string Name
            {
                get { return null; }
            }

            public override int PasswordAttemptWindow
            {
                get { return 3; }
            }

            public override MembershipPasswordFormat PasswordFormat
            {
                get { return MembershipPasswordFormat.Clear; }
            }

            public override string PasswordStrengthRegularExpression
            {
                get { return null; }
            }

            public override bool RequiresQuestionAndAnswer
            {
                get { return false; }
            }

            public override bool RequiresUniqueEmail
            {
                get { return false; }
            }

            public override bool ChangePassword(string username, string oldPassword, string newPassword)
            {
                throw new NotImplementedException();
            }

            public override bool ChangePasswordQuestionAndAnswer(string username, string password,
                                                                 string newPasswordQuestion, string newPasswordAnswer)
            {
                throw new NotImplementedException();
            }

            public override MembershipUser CreateUser(string username, string password, string email,
                                                      string passwordQuestion, string passwordAnswer, bool isApproved,
                                                      Object providerUserKey, out MembershipCreateStatus status)
            {
                var user = new MockMembershipUser();

                if (username.Equals("someUser") && password.Equals("goodPass") && email.Equals("email"))
                {
                    status = MembershipCreateStatus.Success;
                }
                else
                {
                    // the 'email' parameter contains the status we want to return to the user
                    status = (MembershipCreateStatus) Enum.Parse(typeof (MembershipCreateStatus), email);
                }

                return user;
            }

            public override bool DeleteUser(string username, bool deleteAllRelatedData)
            {
                throw new NotImplementedException();
            }

            public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize,
                                                                      out int totalRecords)
            {
                throw new NotImplementedException();
            }

            public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize,
                                                                     out int totalRecords)
            {
                throw new NotImplementedException();
            }

            public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
            {
                throw new NotImplementedException();
            }

            public override int GetNumberOfUsersOnline()
            {
                throw new NotImplementedException();
            }

            public override string GetPassword(string username, string answer)
            {
                throw new NotImplementedException();
            }

            public override string GetUserNameByEmail(string email)
            {
                throw new NotImplementedException();
            }

            public override MembershipUser GetUser(Object providerUserKey, bool userIsOnline)
            {
                throw new NotImplementedException();
            }

            public override MembershipUser GetUser(string username, bool userIsOnline)
            {
                return new MockMembershipUser();
            }

            public override string ResetPassword(string username, string answer)
            {
                throw new NotImplementedException();
            }

            public override bool UnlockUser(string userName)
            {
                throw new NotImplementedException();
            }

            public override void UpdateUser(MembershipUser user)
            {
                throw new NotImplementedException();
            }

            public override bool ValidateUser(string username, string password)
            {
                return password.Equals("goodPass");
            }
        }
    }
}