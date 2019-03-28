using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AdelfyStore.Models;

namespace AdelfyStore.Controllers
{
    [SessionTimeout]
    public class OrderStatusController : Controller
    {
        private SBV_GroupStoreEntities db = new SBV_GroupStoreEntities();

        public ActionResult Index()
        {
            return View(db.TblOrderStatus.Where(e => e.IsDelete == false).ToList());
        }

        // GET: OrderStatus/Create
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TblOrderStatu tblOrderStatu)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.TblOrderStatus.Add(tblOrderStatu);
                    db.SaveChanges();
                    TempData["msg"] = "1";
                    return RedirectToAction("Index");
                }
                TempData["msg"] = "0";
            }
            catch (Exception ex)
            {
                TempData["msg"] = "0";
            }
            return View(tblOrderStatu);
        }

        // GET: OrderStatus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TblOrderStatu tblOrderStatu = db.TblOrderStatus.Find(id);
            if (tblOrderStatu == null)
            {
                return HttpNotFound();
            }
            return View(tblOrderStatu);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TblOrderStatu tblOrderStatu)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(tblOrderStatu).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["msg"] = "2";
                    return RedirectToAction("Index");
                }
                TempData["msg"] = "0";
                return View(tblOrderStatu);
               
            }
            catch (Exception ex)
            {
                TempData["msg"] = "0";
            }
            return View(tblOrderStatu);
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
