using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyClubLib.Models;

namespace MyClub.UI.Controllers
{
    public class LogsController : Controller
    {
        private MyClubEntities db = new MyClubEntities();

        // GET: log
        public ActionResult Index()
        {
            var auditTrails = db.AuditTrails.Include(a => a.Action).Include(a => a.ActionType).Include(a => a.MasterEntity);
            return View(auditTrails.ToList());
        }

        // GET: log/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuditTrail auditTrail = db.AuditTrails.Find(id);
            if (auditTrail == null)
            {
                return HttpNotFound();
            }
            return View(auditTrail);
        }

        // GET: log/Create
        public ActionResult Create()
        {
            ViewBag.ActionId = new SelectList(db.Actions, "ActionId", "ActionName");
            ViewBag.ActionTypeId = new SelectList(db.ActionTypes, "ActionTypeId", "ActionTypeName");
            ViewBag.EntityId = new SelectList(db.MasterEntities, "EntityId", "EntityName");
            return View();
        }

        // POST: log/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AuditId,ActionTypeId,ActionId,UserId,IPAddress,TransactionTime,EntityId,EntityRecord")] AuditTrail auditTrail)
        {
            if (ModelState.IsValid)
            {
                db.AuditTrails.Add(auditTrail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ActionId = new SelectList(db.Actions, "ActionId", "ActionName", auditTrail.ActionId);
            ViewBag.ActionTypeId = new SelectList(db.ActionTypes, "ActionTypeId", "ActionTypeName", auditTrail.ActionTypeId);
            ViewBag.EntityId = new SelectList(db.MasterEntities, "EntityId", "EntityName", auditTrail.EntityId);
            return View(auditTrail);
        }

        // GET: log/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuditTrail auditTrail = db.AuditTrails.Find(id);
            if (auditTrail == null)
            {
                return HttpNotFound();
            }
            ViewBag.ActionId = new SelectList(db.Actions, "ActionId", "ActionName", auditTrail.ActionId);
            ViewBag.ActionTypeId = new SelectList(db.ActionTypes, "ActionTypeId", "ActionTypeName", auditTrail.ActionTypeId);
            ViewBag.EntityId = new SelectList(db.MasterEntities, "EntityId", "EntityName", auditTrail.EntityId);
            return View(auditTrail);
        }

        // POST: log/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AuditId,ActionTypeId,ActionId,UserId,IPAddress,TransactionTime,EntityId,EntityRecord")] AuditTrail auditTrail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(auditTrail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ActionId = new SelectList(db.Actions, "ActionId", "ActionName", auditTrail.ActionId);
            ViewBag.ActionTypeId = new SelectList(db.ActionTypes, "ActionTypeId", "ActionTypeName", auditTrail.ActionTypeId);
            ViewBag.EntityId = new SelectList(db.MasterEntities, "EntityId", "EntityName", auditTrail.EntityId);
            return View(auditTrail);
        }

        // GET: log/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuditTrail auditTrail = db.AuditTrails.Find(id);
            if (auditTrail == null)
            {
                return HttpNotFound();
            }
            return View(auditTrail);
        }

        // POST: log/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AuditTrail auditTrail = db.AuditTrails.Find(id);
            db.AuditTrails.Remove(auditTrail);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
