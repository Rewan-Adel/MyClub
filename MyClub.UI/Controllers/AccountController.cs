using System;
using System.Net;
using System.Web.Mvc;
using WebMatrix.WebData;
using SecurityLib.Repositoty;
using System.Net.Mail;
using System.Web.Security;

namespace MyClub.UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly SecurityRepository _security;
        private readonly int _userId;
        public AccountController()
        {
            _security = new SecurityRepository();
            _userId   = WebSecurity.CurrentUserId;
        }
        public ActionResult Index()
        {

            ViewBag.Title = "Index";

            return View();
        }

        public ActionResult Signup()
        {
            return View();
        }

        public ActionResult forpass()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            ViewBag.Title = "Login";
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(string Email, string password)
        {
            bool IsUser = _security.IsUserExist(Email, password);
            if (IsUser)
            {
                if (!_security.IsActiveUser(Email))
                {
                    ViewBag.Error = "This account is stopped.";
                    return View();
                }
                else
                {
                    Session["login"] = true;
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewBag.Error = "Invalid Email or Password";
                return View();
            }
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Signup(string username, string Gender, string Password, DateTime BirthDay, string MobileNumber, string HomePhoneNumber,
                 string Email, string Address, string Nationality)
        {
            try
            {
                if (!WebSecurity.Initialized)
                {
                    WebSecurity.InitializeDatabaseConnection(
                        connectionStringName: "MyclubSecurity",
                        userTableName: "User_Profile",
                        userIdColumn: "UserId",
                        userNameColumn: "UserName",
                        autoCreateTables: true);
                }

                if (_security.IsUserExist(Email, null))
                {
                    ViewBag.Error = "This email already exists.";
                    return View();
                }

              // _security.Register(null, username, Password, Gender, Address, BirthDay, MobileNumber, HomePhoneNumber, Email, Nationality);
                WebSecurity.CreateUserAndAccount(username, Password);

                Session["login"] = true;

                return RedirectToAction("Index", "Home");
            }
            catch (MembershipCreateUserException e)
            {
                ViewBag.Error = ErrorCodeToString(e.StatusCode);
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Username already exists. Please enter a different username.";
                case MembershipCreateStatus.DuplicateEmail:
                    return "A username for that email address already exists. Please enter a different email address.";
                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password.";
                case MembershipCreateStatus.InvalidEmail:
                    return "The email address provided is invalid. Please check the value and try again.";
                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";
                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";
                case MembershipCreateStatus.InvalidUserName:
                    return "The username provided is invalid. Please check the value and try again.";
                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        public ActionResult Logout()
        {
            WebSecurity.Logout();
            Session["login"] = null;
            return RedirectToAction("Login");
        }

        private string GenerateVerificationCode()
        {
            return new Random().Next(100000, 999999).ToString();
        }

        [HttpPost]
        public ActionResult SendVerificationCode(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.Error = "Email is required.";
                return View("forpass");
            }

            var verificationCode = GenerateVerificationCode();
            Session["VerificationCode"] = verificationCode;

            try
            {
                SendEmail(email, verificationCode);
                ViewBag.Message = "Verification code has been sent to your email.";
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Failed to send verification code. Error: {ex.Message}";
                return View("forpass");
            }

            return View("SendVerificationCode");
        }

        [HttpPost]
        public ActionResult verify(string code)
        {
            var storedCode = Session["VerificationCode"] as string;

            if (string.IsNullOrEmpty(storedCode) || storedCode != code)
            {
                ViewBag.Error = "Invalid verification code. Please try again.";
                return View("verify");
            }

            ViewBag.Message = "Verification code is correct!";
            return Redirect("ResetPassword");
        }

        [HttpPost]
        public ActionResult ResetPassword(string newPassword)
        {
            ViewBag.Message = "Password has been successfully reset.";
            return View("ResetPassword");
        }

        private void SendEmail(string email, string verificationCode)
        {
            var fromAddress = new MailAddress("myclubverificationsender@outlook.com", "MY club");
            var toAddress = new MailAddress(email);
            const string fromPassword = "myclub1Sender";
            const string subject = "Verification Code";
            string body = $"Your verification code is {verificationCode}";

            var smtp = new SmtpClient
            {
                Host = "smtp-mail.outlook.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }


    }
}