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
using System.Linq;
using System.Web.Helpers;
using MyClubLib.Repository;
using utilities = MyClubLib.Repository;
using System.Net.Http.Headers;
namespace MyClub.UI.Controllers
{
    public class MemberController : Controller
    {
        private MyClubEntities _db = new MyClubEntities();
        private readonly SecurityRepository _security;
        private readonly EFClubRepository _entities;
        private readonly int? userId;

        public MemberController()
        {
             userId = WebSecurity.CurrentUserId;
            _security = new SecurityRepository();
            _entities = new EFClubRepository();
        }
        // GET: Member
        public ActionResult Index()
        {
            if (Session["login"] != null)
            {
                return View(_db.People.ToList());
            }
            else
                return RedirectToAction("Login", "Account");    
        }

        // GET: Member/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["login"] != null){
                if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = _db.People.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }
                return View(person);
            }
            else
                return RedirectToAction("Login", "Account");    
        }

        // GET: Member/Create
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PersonName,Password,Gender,BirthDate,MobileNumber,HomePhoneNumber,Email,Address,Nationality")] Person person)
        {
            try
            {
                if (Session["Login"] != null)
                {
                    if (ModelState.IsValid)
                    {
                        var User = _security.IsUserExist(person.Email, person.Password);
                        if (User != null)
                        {
                            ViewBag.Error = "This email or username already exists.";
                            return View();
                        }
                        else
                        {
                            var newPerson = new Person
                            {
                                PersonName = person.PersonName,
                                Password = person.Password,
                                Gender = person.Gender,
                                BirthDate = person.BirthDate,
                                MobileNumber = person.MobileNumber,
                                HomePhoneNumber = person.HomePhoneNumber,
                                Email = person.Email,
                                Address = person.Address,
                                Nationality = person.Nationality,
                                RegistrationDate = DateTime.Now,
                                isExpected = false
                            };
                            _db.People.Add(person);
                            _db.SaveChanges();

                            var member = new Member
                            {
                                MemberName = person.PersonName,
                                PersonId = newPerson.PersonId,
                                RegistrationDate = DateTime.Now
                            };

                            _db.Members.Add(member);
                            _db.SaveChanges();

                            _security.CreateUser(person.Email, true, false);

                            if (userId != null)
                            {
                               var user = _entities.Find<Person>((int)userId);
                             if (user != null)
                            {
                              string entityRecord = $"{user.PersonName} added new member {person.PersonName} to the system.";
                            _entities.CreateAudit(utilities.ActionType.Add, utilities.Action.Create_Member, userId, utilities.MasterEntity.Member, entityRecord);
                            }
                            else
                             {
                                ViewBag.Error = "User not found.";
                            }
                            }

                            return RedirectToAction("Index");
                        }
                    }
                    else return View(person);
                }
                else
                    return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View();
            }
        }
 
        public ActionResult Edit(int? id)
        {
            if (Session["login"] != null)
            {

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Person person = _db.People.Find(id);
                if (person == null)
                {
                    return HttpNotFound();
                }
                return View(person);
            }else
                return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PersonId,PersonName,Password,Gender,BirthDate,MobileNumber,HomePhoneNumber,Email,Address,Nationality,UserId,RegistrationDate,isExpected,MemberOfferId")] Person person)
        {
            if (ModelState.IsValid)
            {
               
                return RedirectToAction("Index");
            }
            return View(person);
        }

        // GET: Member/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = _db.People.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        // POST: Member/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Person person = _db.People.Find(id);
            _db.People.Remove(person);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Clear")]
        public ActionResult Clear()
        {
             if (Session["login"] != null)
             {
               _entities.DeleteAll<Person>();
               _entities.DeleteAll<Member>();
                return RedirectToAction("Index");
              }
             else
              {
                return RedirectToAction("Login", "Account");
              }


        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
