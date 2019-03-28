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
    public class BannersController : Controller
    {
        private SBV_GroupStoreEntities db = new SBV_GroupStoreEntities();

        // GET: Banners
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
              return new HttpStatusCodeResult(HttpStatusCode.BadRequest);               
            }
            else {
                BaseUtil.SetSessionValue(AdminInfo.StoreID.ToString(), id.ToString());
                var tblBanners = db.TblBanners.Where(e => e.IsDelete == false && e.StoreId== id)
                                              .Include(t => t.TblStoreInformation)
                                              .Include(t => t.TblUserInformation)
                                              .Include(t => t.TblUserInformation1)
                                              .OrderByDescending(e => e.CreatedDate);
                return View(tblBanners.ToList());
            }
           
        }



        // GET: Banners/Create
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TblBanner tblBanner, HttpPostedFileBase fileName)
        {
            try
            {
                if (fileName != null)
                {
                    tblBanner.BannerPath = BaseUtil.UploadFile(fileName, "/Content/Images");
                }
                else
                {
                    tblBanner.BannerPath = "/images/master-slide-01.jpg";  
                }
                tblBanner.CreatedBy          = Convert.ToInt32(BaseUtil.GetSessionValue(AdminInfo.LoginID.ToString()));
                tblBanner.CreatedDate = DateTime.Now;
                tblBanner.ModifiedBy = Convert.ToInt32(BaseUtil.GetSessionValue(AdminInfo.LoginID.ToString()));
                tblBanner.ModifiedDate = DateTime.Now;
                tblBanner.IsActive = true;
                tblBanner.IsDelete = false;
                tblBanner.SrNumber = 1;
                tblBanner.StoreId = Convert.ToInt32(BaseUtil.GetSessionValue(AdminInfo.StoreID.ToString()));
                if (ModelState.IsValid)
                {
                    db.TblBanners.Add(tblBanner);
                    db.SaveChanges();
                    TempData["msg"] = "1";
                    return RedirectToAction("Index",new { id= tblBanner .StoreId});
                }
                TempData["msg"] = "0";
            }
            catch (Exception ex)
            {
                TempData["msg"] = "0";
            }
            return View(tblBanner);
        }

        // GET: Banners/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TblBanner tblBanner = db.TblBanners.Find(id);
            if (tblBanner == null)
            {
                return HttpNotFound();
            }
           

            return View(tblBanner);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TblBanner tblBanner, HttpPostedFileBase fileName)
        {
            try
            {
                tblBanner.StoreId = Convert.ToInt32(BaseUtil.GetSessionValue(AdminInfo.StoreID.ToString()));
                tblBanner.ModifiedBy = Convert.ToInt32(BaseUtil.GetSessionValue(AdminInfo.LoginID.ToString()));
                tblBanner.ModifiedDate = DateTime.Now;
                if (fileName != null)
                {                   
                    tblBanner.BannerPath = BaseUtil.UploadFile(fileName, "/Content/Images");
                }
                if (ModelState.IsValid)
                {
                    db.Entry(tblBanner).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["msg"] = "2";
                    return RedirectToAction("Index", new { id = tblBanner.StoreId });
                }
                TempData["msg"] = "0";
            }
            catch (Exception ex)
            {
                TempData["msg"] = "0";
            }
            return View(tblBanner);
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
