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
    public class CompanyTypesController : Controller
    {
        private SBV_GroupStoreEntities db = new SBV_GroupStoreEntities();

        // GET: CompanyTypes
        public ActionResult Index()
        {
            return View(db.TblCompanyTypes.Where(e=>e.IsActive==true).ToList());
        }

     
        // GET: CompanyTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TblCompanyType tblCompanyType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    tblCompanyType.IsActive = true;
                    db.TblCompanyTypes.Add(tblCompanyType);
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
            return View(tblCompanyType);
        }

        // GET: CompanyTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TblCompanyType tblCompanyType = db.TblCompanyTypes.Find(id);
            if (tblCompanyType == null)
            {
                return HttpNotFound();
            }
            return View(tblCompanyType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CompanyTypeID,CompanyType,IsActive")] TblCompanyType tblCompanyType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(tblCompanyType).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["msg"] = "2";
                    return RedirectToAction("Index");
                }
                TempData["msg"] = "0";
            }
            catch (Exception ex) {
                TempData["msg"] = "0";
            }            
            return View(tblCompanyType);
                
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
