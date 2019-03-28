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
    public class ProductCategoriesController : Controller
    {
        private SBV_GroupStoreEntities db = new SBV_GroupStoreEntities();

        // GET: ProductCategories
        public ActionResult Index()
        {
            return View(db.TblProductCategories.Where(e=>e.IsDelete==false).ToList());
        }
        // GET: ProductCategories/Create
        public ActionResult Create()
        {
            return View();
        }
                
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TblProductCategory tblProductCategory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.TblProductCategories.Add(tblProductCategory);
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
            return View(tblProductCategory);
        }

        // GET: ProductCategories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TblProductCategory tblProductCategory = db.TblProductCategories.Find(id);
            if (tblProductCategory == null)
            {
                return HttpNotFound();
            }
            return View(tblProductCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TblProductCategory tblProductCategory)
        {
            try { 
            if (ModelState.IsValid)
            {
                db.Entry(tblProductCategory).State = EntityState.Modified;
                db.SaveChanges();
                TempData["msg"] = "2";
                return RedirectToAction("Index");
            }
            TempData["msg"] = "0";
            }
            catch (Exception ex)
            {
                TempData["msg"] = "0";
            }
            return View(tblProductCategory);
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
