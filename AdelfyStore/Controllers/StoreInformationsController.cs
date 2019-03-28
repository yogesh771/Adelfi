using System;
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
    public class StoreInformationsController : Controller
    {
        private SBV_GroupStoreEntities db = new SBV_GroupStoreEntities();

        // GET: StoreInformations
        public ActionResult Index()
        {
            var tblStoreInformations = db.TblStoreInformations.Where(e=>e.IsDelete==false).Include(t => t.TblCompanyType).Include(t => t.TblTheme).OrderByDescending(e=>e.CreatedDate);
            return View(tblStoreInformations.ToList());
        }

        // GET: StoreInformations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TblStoreInformation tblStoreInformation = db.TblStoreInformations.Find(id);
            if (tblStoreInformation == null)
            {
                return HttpNotFound();
            }
            BaseUtil.SetSessionValue(AdminInfo.StoreID.ToString(), id.ToString());
            return View(tblStoreInformation);
        }

        // GET: StoreInformations/Create
        public ActionResult Create()
        {
            ViewBag.CompanyTypeID = db.TblCompanyTypes.Where(e=>e.IsActive==true).Select(e => new { e.CompanyTypeID, e.CompanyType }).OrderBy(e=>e.CompanyType);
            return View();
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TblStoreInformation tblStoreInformation, HttpPostedFileBase  fileName)
        {
            try
            {
                ViewBag.CompanyTypeID = new SelectList(db.TblCompanyTypes.Where(e => e.IsActive == true), "CompanyTypeID", "CompanyType", tblStoreInformation.CompanyTypeID);
                tblStoreInformation.IsActive = true;
                tblStoreInformation.IsDelete = false;
                tblStoreInformation.ModifiedBy = Convert.ToInt32(BaseUtil.GetSessionValue(AdminInfo.LoginID.ToString())); ;
                tblStoreInformation.ModifiedDate = DateTime.Now;
                tblStoreInformation.CreatedBy = Convert.ToInt32(BaseUtil.GetSessionValue(AdminInfo.LoginID.ToString()));
                tblStoreInformation.CreatedDate = DateTime.Now;
                tblStoreInformation.ThemeID = 1;
                var price = db.TblStoreFees.FirstOrDefault();
                tblStoreInformation.storeCreationFee = price.Fee;
                if (ModelState.IsValid)
                {
                    if (fileName != null)
                    {
                        tblStoreInformation.Logo = BaseUtil.UploadFile(fileName, "/Content/Images");
                    }
                    else {
                        tblStoreInformation.Logo = "/Content/Images/emptyLogo.jpg";
                    }
                    db.TblStoreInformations.Add(tblStoreInformation);
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

            return View(tblStoreInformation);
        }

        // GET: StoreInformations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TblStoreInformation tblStoreInformation = db.TblStoreInformations.Find(id);
            if (tblStoreInformation == null)
            {
                return HttpNotFound();
            }
            BaseUtil.SetSessionValue(AdminInfo.StoreID.ToString(), id.ToString());
            ViewBag.CompanyTypeID = new SelectList(db.TblCompanyTypes.Where(e => e.IsActive == true), "CompanyTypeID", "CompanyType", tblStoreInformation.CompanyTypeID);           
            return View(tblStoreInformation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TblStoreInformation tblStoreInformation, HttpPostedFileBase fileName)
        {
            tblStoreInformation.ModifiedBy = Convert.ToInt32(BaseUtil.GetSessionValue(AdminInfo.LoginID.ToString()));
            tblStoreInformation.ModifiedDate = DateTime.Now;
            try
            {
                if (fileName != null)
                {
                    //BaseUtil.DeleteAlreadyExistingFile(tblStoreInformation.Logo, "/Content/Images");
                    tblStoreInformation.Logo = tblStoreInformation.Logo = BaseUtil.UploadFile(fileName, "/Content/Images");  
                }
                ViewBag.CompanyTypeID = new SelectList(db.TblCompanyTypes.Where(e => e.IsActive == true), "CompanyTypeID", "CompanyType", tblStoreInformation.CompanyTypeID);
                if (ModelState.IsValid)
                {
                    db.Entry(tblStoreInformation).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["msg"] = "2";
                    return RedirectToAction("Index");
                }           
           
             TempData["msg"] = "0";
            }
            catch (Exception ex) {
                TempData["msg"] = "0";
            }  
            return View(tblStoreInformation);
        }

        
       public JsonResult IsServiceEmailExists(string ServiceEmail, string PreviousServiceEmail)
        {
            if (ServiceEmail == PreviousServiceEmail)
                return Json(true, JsonRequestBehavior.AllowGet);
            else
            {
                bool found = db.TblStoreInformations.Any(e => e.ServiceEmail == ServiceEmail);
                if (!found)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult IsServicePhoneExists(string ServicePhone, string PreviousServicePhone)
        {
            if (ServicePhone == PreviousServicePhone)
                return Json(true, JsonRequestBehavior.AllowGet);
            else
            {
                bool found = db.TblStoreInformations.Any(e => e.ServicePhone == ServicePhone);
                if (!found)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult IsStoreUrlExists(string StoreURL, string PreviousStoreURL)
        {
            if (StoreURL == PreviousStoreURL)
                return Json(true, JsonRequestBehavior.AllowGet);
            else
            {
                bool found = db.TblStoreInformations.Any(e => e.StoreURL == StoreURL);
                if (!found)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult accounts()
        {
            var totalPrice = 0.0;
            var store = db.TblStoreInformations.OrderByDescending(e=>e.CreatedDate).ToList();
            foreach (var fee in store)
            {
                totalPrice = totalPrice + Convert.ToDouble(fee.storeCreationFee);
            }
            ViewBag.totalPrice = totalPrice;
            return View(store);
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
