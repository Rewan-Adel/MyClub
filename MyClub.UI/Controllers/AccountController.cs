using System;
using System.Web.Mvc;
using WebMatrix.WebData;
using SecurityLib.Repositoty;

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

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
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











    }
}