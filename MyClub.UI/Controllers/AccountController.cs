using System;
using System.Net;
using System.Web.Mvc;
using WebMatrix.WebData;
using SecurityLib.Repositoty;
using System.Net.Mail;

namespace MyClub.UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly SecurityRepository _security;
        //private readonly int _userId;
        public AccountController()
        {
            _security = new SecurityRepository();
            //_userId   = WebSecurity.CurrentUserId;
        }
        public ActionResult Index()
        {
            ViewBag.Title = "Index";

            return View();
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public ActionResult forpass()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Signup(string PersonName, string Gender, string Password, DateTime BirthDay, string MobileNumber, string HomePhoneNumber,
                                 string Email, string Address, string Nationality)
        {
            try
            {
                _security.Register(null, PersonName, Password, Gender, Address, BirthDay, MobileNumber, HomePhoneNumber, Email, Address, Nationality);      

                Session["login"] = true;
                
                return RedirectToAction("Index");
               // return Json(new { success = true, message = "Signup successfully." }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
      
      //  [HttpPost]
      //  [AllowAnonymous]
        //public ActionResult Login(LoginModel model)
       // {
       //     if(ModelState.IsValid && WebSecurity.Login(model.userName, model.password))
        //    {
        //        if(!_security.checkUserStatus(model.userName))
           //     {
           //         ModelState.AddModelError("", "This account is stopped.");
           //     }
           //     else
          //      {
           //         Session["login"] = true;
          //          return RedirectToAction("Index" , "Member");
          //      }
         //   }else
          //  {
          //      return View();
          //  }
       // }
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