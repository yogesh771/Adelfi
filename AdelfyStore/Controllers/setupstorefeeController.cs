using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AdelfyStore.Models;

namespace AdelfyStore.Controllers
{
    [SessionTimeout]
    [RoutePrefix("setup-store-fee")]
    public class setupstorefeeController : Controller
    {
        private SBV_GroupStoreEntities db = new SBV_GroupStoreEntities();

        // GET: setupstorefee
        public ActionResult Index()
        {
            return View(db.TblStoreFees.ToList());
        }

     

        // GET: setupstorefee/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TblStoreFee tblStoreFee = db.TblStoreFees.Find(id);
            if (tblStoreFee == null)
            {
                return HttpNotFound();
            }
            return View(tblStoreFee);
        }

        // POST: setupstorefee/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,Fee")] TblStoreFee tblStoreFee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblStoreFee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblStoreFee);
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
