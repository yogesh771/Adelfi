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
    public class ReportOrderHistoriesController : Controller
    {
        private SBV_GroupStoreEntities db = new SBV_GroupStoreEntities();

        // GET: ReportOrderHistories
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                BaseUtil.SetSessionValue(AdminInfo.StoreID.ToString(), id.ToString());
                var tblOrderHistories = db.TblOrderHistories.Where(e=>e.StoreId== id)
                                                            .Include(t => t.TblOrderStatu)
                                                            .Include(t => t.TblProduct);
                return View(tblOrderHistories.ToList());
            }
        }
        public ActionResult Orders()
        {               
                var tblOrderHistories = db.TblOrderHistories.Include(t => t.TblOrderStatu)
                                                            .Include(t=>t.TblStoreInformation)
                                                            .Include(t => t.TblProduct);
                return View(tblOrderHistories.ToList());
            
        }
        public ActionResult UserAccounts(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                BaseUtil.SetSessionValue(AdminInfo.StoreID.ToString(), id.ToString());
                var tblOrderHistories = db.TblOrderHistories.Distinct();
                return View(tblOrderHistories.ToList());
            }
        }

        // GET: ReportOrderHistories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TblOrderHistory tblOrderHistory = db.TblOrderHistories.Find(id);
            if (tblOrderHistory == null)
            {
                return HttpNotFound();
            }
            return View(tblOrderHistory);
        }
        public ActionResult changeOrderStatus(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var orderHistory = db.TblOrderHistories.Find(id);
            if (orderHistory == null)
            {
                return HttpNotFound();
            }
            orderHistoryStatus orderHistoryStatus = new orderHistoryStatus();

            orderHistoryStatus.OrderID = Convert.ToInt32( id);
            orderHistoryStatus.OrderStatusID = orderHistory.OrderStatusID;
            ViewBag.OrderStatusID = new SelectList(db.TblOrderStatus.Where(e => e.IsDelete == false).ToList(), "OrderStatusID", "Status", orderHistory.OrderStatusID);
            return View(orderHistoryStatus);
        }
        [HttpPost]
        public ActionResult changeOrderStatus(orderHistoryStatus orderHistoryStatus)
        {
            try { 
            ViewBag.OrderStatusID = new SelectList(db.TblOrderStatus.Where(e => e.IsDelete == false).ToList(), "OrderStatusID", "Status", orderHistoryStatus.OrderStatusID);

            var order = db.TblOrderHistories.Find(orderHistoryStatus.OrderID);
            order.OrderStatusID = orderHistoryStatus.OrderStatusID;
                order.ModifiedBy = Convert.ToInt32(BaseUtil.GetSessionValue(AdminInfo.LoginID.ToString()));
                order.ModifiedDate = DateTime.Now;
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();
            TempData["msg"] = "2";
            return RedirectToAction("Details", new { id= order.OrderID});        
            }
            catch (Exception ex) {
                TempData["msg"] = "0";
            }  
            return View(orderHistoryStatus);
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
