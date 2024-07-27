using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyClubLib.Models;
using MyClubLib.Repository;
using WebMatrix.WebData;

namespace MyClub.UI.Controllers
{
    public class MemberOfferController : Controller
    {
        private MyClubEntities db = new MyClubEntities();
        private readonly EFClubRepository _db;

        private readonly int userId;

        public MemberOfferController()
        {
            _db = new EFClubRepository();
            userId = WebSecurity.CurrentUserId;
        }
        public ActionResult Index()
        {
            if (Session["Login"] != null)
            {
                var memberOffers = _db.GetAll<MemberOffer>();
                return View(memberOffers);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }

        public ActionResult Details(int? id)
        {
            if (Session["Login"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                MemberOffer memberOffer = _db.Find<MemberOffer>((int)id);
                if (memberOffer == null)
                {
                    return HttpNotFound();
                }
                return View(memberOffer);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }
        public ActionResult Create()
        {
            if (Session["Login"] != null)
            {
                ViewBag.MemberId = new SelectList(db.Members, "MemberId", "MemberName");
                ViewBag.OfferPriceId = new SelectList(db.OfferPrices, "OfferPriceId", "OfferPriceId");
                ViewBag.TrainerId = new SelectList(db.People, "PersonId", "PersonName");
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MemberOfferId,MemberId,OfferPriceId,PaymentAmount,PaymentDate,CurrentStatusId,CreatedById,CreationDate,LastModifiedDate,MemberPrice,DiscountPercent,DiscountValue,DiscountById,EndDate,Note,TrainerId")] MemberOffer memberOffer)
        {
            if (Session["Login"] != null)
            {
                if (ModelState.IsValid)
                {

                    MemberOffer newMemberOffer = _db.CreateMemberOffer((int)memberOffer.MemberId, memberOffer.Note, memberOffer.OfferPriceId, memberOffer.PaymentAmount,
                        offerStatus.Active, userId, memberOffer.MemberPrice, memberOffer.DiscountPercent, memberOffer.DiscountValue, memberOffer.DiscountById, memberOffer.TrainerId);
               
                    //db.MemberOffers.Add(memberOffer);
                    //db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.MemberId = new SelectList(db.Members, "MemberId", "MemberName", memberOffer.MemberId);
                ViewBag.OfferPriceId = new SelectList(db.OfferPrices, "OfferPriceId", "OfferPriceId", memberOffer.OfferPriceId);
                return View(memberOffer);
            }

            else
            {
                return RedirectToAction("Login", "Account");
            }
        } 
   

        // GET: MemberOffer/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MemberOffer memberOffer = _db.Find<MemberOffer>((int)id);
            if (memberOffer == null)
            {
                return HttpNotFound();
            }
            ViewBag.MemberId = new SelectList(db.Members, "MemberId", "MemberName", memberOffer.MemberId);
            ViewBag.OfferPriceId = new SelectList(db.OfferPrices, "OfferPriceId", "OfferPriceId", memberOffer.OfferPriceId);
            return View(memberOffer);
        }

        // POST: MemberOffer/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MemberOfferId,MemberId,OfferPriceId,PaymentAmount,PaymentDate,CurrentStatusId,CreatedById,CreationDate,LastModifiedDate,MemberPrice,DiscountPercent,DiscountValue,DiscountById,EndDate,Note,TrainerId")] MemberOffer memberOffer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(memberOffer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MemberId = new SelectList(db.Members, "MemberId", "MemberName", memberOffer.MemberId);
            ViewBag.OfferPriceId = new SelectList(db.OfferPrices, "OfferPriceId", "OfferPriceId", memberOffer.OfferPriceId);
            return View(memberOffer);
        }

        // GET: MemberOffer/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MemberOffer memberOffer = db.MemberOffers.Find(id);
            if (memberOffer == null)
            {
                return HttpNotFound();
            }
            return View(memberOffer);
        }

        // POST: MemberOffer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MemberOffer memberOffer = db.MemberOffers.Find(id);
            db.MemberOffers.Remove(memberOffer);
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
