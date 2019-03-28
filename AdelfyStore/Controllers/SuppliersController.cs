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
    public class SuppliersController : Controller
    {
        private SBV_GroupStoreEntities db = new SBV_GroupStoreEntities();

        // GET: Suppliers
        public ActionResult Index()
        {
            var tblSuppliers = db.TblSuppliers.Where(e => e.IsDelete == false).Include(t => t.TblUserInformation).Include(t => t.TblUserInformation1).OrderBy(e => e.SupplierName);
            return View(tblSuppliers.ToList());
        }


        // GET: Suppliers/Create
        public ActionResult Create()
        {           
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TblSupplier tblSupplier)
        {
            try
            {               
                tblSupplier.IsActive = true;
                tblSupplier.IsDelete = false;
                tblSupplier.ModifiedBy = Convert.ToInt32(BaseUtil.GetSessionValue(AdminInfo.LoginID.ToString()));
                tblSupplier.CreatedBy = Convert.ToInt32(BaseUtil.GetSessionValue(AdminInfo.LoginID.ToString()));
                tblSupplier.ModifiedDate = DateTime.Now;
                tblSupplier.CreatedDate = DateTime.Now;
                if (ModelState.IsValid)
                {
                    db.TblSuppliers.Add(tblSupplier);
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

            return View(tblSupplier);
        }

        // GET: Suppliers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TblSupplier tblSupplier = db.TblSuppliers.Find(id);
            if (tblSupplier == null)
            {
                return HttpNotFound();
            }          

            return View(tblSupplier);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( TblSupplier tblSupplier)
        {
            try
            {             
                tblSupplier.ModifiedBy = Convert.ToInt32(BaseUtil.GetSessionValue(AdminInfo.LoginID.ToString()));
                tblSupplier.ModifiedDate = DateTime.Now;
                              
                if (ModelState.IsValid)
                {
                    db.Entry(tblSupplier).State = EntityState.Modified;
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
            return View(tblSupplier);
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
