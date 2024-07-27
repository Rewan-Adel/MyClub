using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using MyClubLib.Models;
using MyClubLib.Repository;
using WebMatrix.WebData;
namespace MyClub.UI.Controllers
{
    public class ServiceController : Controller
    {
        private MyClubEntities db = new MyClubEntities();

        private readonly EFClubRepository _db;
        private readonly int userId;
        public ServiceController()
        {

            _db = new EFClubRepository();
            userId = WebSecurity.CurrentUserId;
        }
        public ActionResult Index()
        {
            if (Session["login"] != null)
            {
                var services = _db.GetAllService();
                return View(services);
            } 
              else
             {
            return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult Details(int? id)
        {
            // if (Session["login"] != null)
            // {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            service service = _db.Find<service>((int)id);
            if (service == null)
            {
                return HttpNotFound();
            }
            return View(service);
            //  }
            // else
            // {
            return RedirectToAction("Login", "Account");
            // }

        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ServiceName,ServicePrice,IsActive")] service service)
        {
            if (Session["login"] != null)
            {
                if (ModelState.IsValid)
                {
                    _db.CreateService(userId, service.ServiceName, (int)service.ServicePrice, (bool)service.IsActive);

                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            return View(service);
        }
        public ActionResult Edit(int? id)
        {
            if (Session["login"] != null)
            {
                 if (id == null)
                  return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                 
                 service service = _db.Find<service>((int)id);
                 if (service == null)
                       return HttpNotFound();
                  
                return View(service);
            }
            else
                return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ServiceId,ServiceName,ServicePrice,IsActive,CreationDate,LastModifiedDate,IsDeleted,IsDefault")] service service)
        {
            if (Session["login"] != null)
            {
                if (ModelState.IsValid)
                {
                    db.Entry(service).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else return View(service);

            }
            else
             {
               return RedirectToAction("Login", "Account");
             }
         }
        public ActionResult Delete(int? id)
        {
            if (Session["login"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                service service = _db.Find<service>((int)id);
                if (service == null)
                {
                    return HttpNotFound();
                }
                return View(service);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["login"] != null)
            {
                service service = db.services.Find(id);
            db.services.Remove(service);
            db.SaveChanges();

            return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost, ActionName("Clear")]
        public ActionResult Clear()
        {
           if (Session["login"] != null)
           {
                _db.DeleteAll<service>();
                return RedirectToAction("Index");
            }
           else
           {
               return RedirectToAction("Login", "Account");
          }
        }

       
    }
}
