using System;
using System.Net;
using System.Web.Mvc;
using WebMatrix.WebData;
using SecurityLib.Repositoty;
using System.Net.Mail;
using System.Web.Security;
using MyClubLib.Models;
using static SecurityLib.Repositoty.SecurityRepository;
using System.Threading.Tasks;

namespace MyClub.UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly SecurityRepository _security;
        private readonly MyClubEntities _context;
        private readonly int _userId;
        public AccountController()
        {
            _context = new MyClubEntities();
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
        [ValidateAntiForgeryToken]
        public ActionResult Login(string Email, string password)
        {
            int? User = _security.IsUserExist(Email, password);
            if (User != null)
            {
                if (!_security.IsActiveUser(Email))
                {
                    ViewBag.Error = "This account is stopped.";
                    return View();
                }
   
                Session["login"] = User;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Error = "Invalid Email or Password";
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Signup(string PersonName, string Gender, string Password, DateTime BirthDate, string MobileNumber, string HomePhoneNumber,
                 string Email, string Address, string Nationality)
        {
            try {
                var User = _security.IsUserExist(Email, Password);
                if (User != null) 
                {
                    ViewBag.Error = "This email or username already exists.";
                    return View();
                }
                else
                {
                    var person = new Person
                    {
                        PersonName = PersonName,
                        Password =  Password,                        Gender = Gender,
                        BirthDate = BirthDate,
                        MobileNumber = MobileNumber,
                        HomePhoneNumber = HomePhoneNumber,
                        Email = Email,
                        Address = Address,
                        Nationality = Nationality,
                        RegistrationDate = DateTime.Now,
                        isExpected = false
                    };
                    _context.People.Add(person);
                    await _context.SaveChangesAsync();
                    var member = new Member
                    {
                        MemberName = PersonName,
                        PersonId = person.PersonId,
                        RegistrationDate = DateTime.Now
                    };

                    _context.Members.Add(member);
                    await _context.SaveChangesAsync();

                   
                    _security.CreateUser(person.Email, true, false);

                    Session["login"] = User;
                    WebSecurity.CreateUserAndAccount(PersonName, Password);
                    return RedirectToAction("Index", "Home"); ;
                }
            }
            catch(Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View();
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
                ViewBag.Error = "Failed to send verification code";
                return View("forpass");
            }

            return View("VerificationCodeSent");
        }

        [HttpPost]
        public ActionResult VerifyCode(string code)
        {
            var sessionCode = Session["VerificationCode"]?.ToString();

            if (sessionCode == null || code != sessionCode)
            {
                ViewBag.Error = "Invalid verification code.";
                return View("VerificationCodeSent");
            }

            ViewBag.Message = "Verification successful. You can now reset your password.";
            return View("ResetPassword");
        }

        [HttpPost]
        public ActionResult ResetPassword(string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                return View();
            }

            // Add your logic to reset the password here

            ViewBag.Message = "Password has been reset successfully.";
            return View();
        }

        private void SendEmail(string email, string verificationCode)
        {
            var fromAddress = new MailAddress("myclubverificationsender2@outlook.com", "MY club");
            var toAddress = new MailAddress(email);
            const string fromPassword = "myclub2Sender";
            const string subject = "Verification Code";
            string body = $"Your verification code is {verificationCode} please don't share it with anyone";

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