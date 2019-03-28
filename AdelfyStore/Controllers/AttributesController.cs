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
    public class AttributesController : Controller
    {
        private SBV_GroupStoreEntities db = new SBV_GroupStoreEntities();

        // GET: Attributes
        public ActionResult Index()
        {
            return View(db.TblAttributes.Where(e=>e.IsDelete==false).ToList());
        }       
        // GET: Attributes/Create
        public ActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TblAttribute tblAttribute)
        {
            try { 
            if (ModelState.IsValid)
            {
                db.TblAttributes.Add(tblAttribute);
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
            return View(tblAttribute);
        }

        // GET: Attributes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TblAttribute tblAttribute = db.TblAttributes.Find(id);
            if (tblAttribute == null)
            {
                return HttpNotFound();
            }            
           
            return View(tblAttribute);
        }
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( TblAttribute tblAttribute)
        {
            try { 
            if (ModelState.IsValid)
            {
                db.Entry(tblAttribute).State = EntityState.Modified;
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
                return View(tblAttribute);
               
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
