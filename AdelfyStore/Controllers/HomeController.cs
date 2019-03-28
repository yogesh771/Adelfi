using AdelfyStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace AdelfyStore.Controllers
{
    [RoutePrefix("adelfi")]
    public class HomeController : Controller
    {
        private SBV_GroupStoreEntities db = new SBV_GroupStoreEntities();
        [Route("dashborard")]
        public ActionResult Index()
        {
            return View();
        }
        [Route("admin")]
        public ActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        [Route("admin")]
        public ActionResult Login(UserLogin userLogin)
        {
            try
            {
                var user = db.TblUserInformations.Where(e => e.UserName == userLogin.UserName).FirstOrDefault();
                if (user != null && user.Password == userLogin.Password)
                {
                    BaseUtil.SetSessionValue(AdminInfo.LoginID.ToString(), user.UserId.ToString());
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.msg = "User name and password does not exists.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.msg = "User sign in Error";
            }
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult SignOut()
        {
            Session.Abandon();
            return RedirectToAction("Index","Client");
        }

        public ActionResult Partial_NewAccounts()
        {
            var sp_Accounts = db.sp_Accounts(null).Take(10).ToList();           
            return PartialView("Partial_NewAccounts", sp_Accounts);
        }
        public ActionResult NewAccounts()
        {
            var sp_Accounts = db.sp_Accounts(null).ToList();
            return View(sp_Accounts);
        }
        public ActionResult Partial_OrderHistory()
        {
            var tblOrderHistories = db.TblOrderHistories.Include(t => t.TblOrderStatu)
                                                           .Include(t => t.TblStoreInformation)
                                                           .Include(t => t.TblProduct);
            
            return PartialView("Partial_OrderHistory", tblOrderHistories.Take(20).ToList());
        }
    }
}