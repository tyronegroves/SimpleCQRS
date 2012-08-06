using System;
using System.Globalization;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using NerdDinner.Commands;
using NerdDinner.Models;
using NerdDinner.Services;

namespace NerdDinner.Controllers
{
    [HandleErrorWithELMAH]
    public class AccountController : Controller
    {
        private readonly CommandServiceClient commandService = new CommandServiceClient();
        private readonly IFormsAuthentication formsAuthentication = new FormsAuthenticationService();
        private readonly MembershipReadModel membershipReadModel = new MembershipReadModel();

        public ActionResult LogOn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(string userName, string password, bool rememberMe, string returnUrl)
        {
            if(!ValidateLogOn(userName, password))
            {
                ViewData["rememberMe"] = rememberMe;
                return View();
            }

            var canonicalUsername = membershipReadModel.GetCanonicalUsername(userName);
            var userId = membershipReadModel.GetUserIdByUserName(userName);
            AddAuthenticationTicketToCookie(userId, userName, rememberMe, canonicalUsername);

            if(!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult LogOff()
        {
            formsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register()
        {
            ViewData["PasswordLength"] = membershipReadModel.MinPasswordLength;
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterUserCommand registerUserCommand, string confirmPassword)
        {
            ViewData["PasswordLength"] = membershipReadModel.MinPasswordLength;

            if(ValidateRegisterUserCommand(registerUserCommand, confirmPassword))
            {
                var userName = registerUserCommand.UserName;
                var userId = registerUserCommand.UserId;
                var createStatus = commandService.RegisterUser(registerUserCommand);
                if (createStatus == MembershipCreateStatus.Success)
                {
                    AddAuthenticationTicketToCookie(userId, userName, false, userName);
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("_FORM", ErrorCodeToString(createStatus));
            }

            return View();
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            ViewData["PasswordLength"] = membershipReadModel.MinPasswordLength;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(string newPassword, string currentPassword, string confirmPassword)
        {
            ViewData["PasswordLength"] = membershipReadModel.MinPasswordLength;

            if (!ValidateChangePassword(currentPassword, newPassword, confirmPassword))
            {
                return View();
            }

            try
            {
                var userId = ((NerdIdentity)User.Identity).UserId;
                var changePasswordCommand = new ChangePasswordCommand{NewPassword = newPassword, UserId = userId};
                if(commandService.ChangePassword(changePasswordCommand))
                    return RedirectToAction("ChangePasswordSuccess");

                ModelState.AddModelError("_FORM", "The current password is incorrect or the new password is invalid.");
                return View();
            }
            catch
            {
                ModelState.AddModelError("_FORM", "The current password is incorrect or the new password is invalid.");
                return View();
            }
        }

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity is WindowsIdentity)
            {
                throw new InvalidOperationException("Windows authentication is not supported.");
            }
        }

        private void AddAuthenticationTicketToCookie(Guid userId, string userName, bool rememberMe, string canonicalUsername)
        {
            var authTicket = new FormsAuthenticationTicket(1, canonicalUsername, DateTime.Now, DateTime.Now.AddMinutes(30),
                                                           rememberMe, string.Join("|", userName, userId));

            var encTicket = FormsAuthentication.Encrypt(authTicket);
            Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
        }

        private bool ValidateChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrEmpty(currentPassword))
            {
                ModelState.AddModelError("currentPassword", "You must specify a current password.");
            }
            if (newPassword == null || newPassword.Length < membershipReadModel.MinPasswordLength)
            {
                ModelState.AddModelError("newPassword",
                    string.Format(CultureInfo.CurrentCulture,
                         "You must specify a new password of {0} or more characters.",
                         membershipReadModel.MinPasswordLength));
            }

            if (!string.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError("_FORM", "The new password and confirmation password do not match.");
            }

            return ModelState.IsValid;
        }

        private bool ValidateRegisterUserCommand(RegisterUserCommand registerUserCommand, string confirmPassword)
        {
            var userName = registerUserCommand.UserName;
            var password = registerUserCommand.Password;
            var email = registerUserCommand.Email;

            if (String.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError("username", "You must specify a username.");
            }
            if (String.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("email", "You must specify an email address.");
            }
            if (password == null || password.Length < membershipReadModel.MinPasswordLength)
            {
                ModelState.AddModelError("password",
                    String.Format(CultureInfo.CurrentCulture,
                         "You must specify a password of {0} or more characters.",
                         membershipReadModel.MinPasswordLength));
            }
            if (!String.Equals(password, confirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError("_FORM", "The new password and confirmation password do not match.");
            }
            return ModelState.IsValid;
        }

        private bool ValidateLogOn(string userName, string password)
        {
            if(String.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError("username", "You must specify a username.");
            }
            if(String.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("password", "You must specify a password.");
            }
            if(!membershipReadModel.ValidateUser(userName, password))
            {
                ModelState.AddModelError("_FORM", "The username or password provided is incorrect.");
            }

            return ModelState.IsValid;
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://msdn.microsoft.com/en-us/library/system.web.security.membershipcreatestatus.aspx for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Username already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A username for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
    }
}