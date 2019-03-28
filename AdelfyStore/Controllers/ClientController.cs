using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AdelfyStore.Models;


namespace AdelfyStore.Controllers
{
    public class ClientController : Controller
    {
        private SBV_GroupStoreEntities db = new SBV_GroupStoreEntities();
        // GET: Client
        private string geturl()
        {
            var url = BaseUtil.GetApplicationPath();// "http://www.streetpluschicago.crastores.com";// 
            var url2 = "";
            if (url == "http://localhost:50279")
            {
                url2 = url;
            }
           else if (url.IndexOf(".") > 0)
            {
                var arr = url.Split('.');
                if (arr[0].Contains("www"))
                {
                    url2 = arr[1];
                }
                else {
                     url2 = arr[0].Substring(7, arr[0].Length - 7);
                }
                
            }
            return url2;
        }
        public ActionResult Index()
        {
            //var url = BaseUtil.GetApplicationPath(); //"http://www.streetpluschicago.crastores.com";//
            if (geturl() == "http://localhost:50279")
            {
                BaseUtil.SetSessionValue(AdminInfo.StoreID.ToString(),"6");
                return View(products(6));
            }
           else 
            {

                var url2 = geturl();
                if (url2 == "crastores")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    var StoreInformation = db.TblStoreInformations.Where(e => e.IsActive == true && e.IsDelete == false && e.StoreURL == url2).OrderByDescending(e => e.CreatedDate).FirstOrDefault();

                    if (StoreInformation == null)
                    {
                        return RedirectToAction("StoreNotAvailable");
                    }
                    BaseUtil.SetSessionValue(AdminInfo.StoreID.ToString(), StoreInformation.StoreId.ToString());
                    BaseUtil.SetSessionValue(AdminInfo.ServiceEmail.ToString(), StoreInformation.ServiceEmail.ToString());
                    BaseUtil.SetSessionValue(AdminInfo.ServicePhone.ToString(), StoreInformation.ServicePhone.ToString());
                    BaseUtil.SetSessionValue(AdminInfo.Logo.ToString(), StoreInformation.Logo.ToString());
                    BaseUtil.SetSessionValue(AdminInfo.storeName.ToString(), StoreInformation.StoreName.ToString());
                    return View(products(StoreInformation.StoreId));
                }
            }            
            
        }
        private List<TblProduct> products(int StoreId)
        {
            
            var banner = db.TblBanners.Where(e => e.IsActive == true && e.IsDelete == false && e.StoreId == StoreId)
                                      .OrderByDescending(e => e.CreatedDate)
                                      .FirstOrDefault();
            if (banner == null)
            {
                ViewBag.banner = "/images/master-slide-01.jpg";
            }
            else
            {
                ViewBag.banner = banner.BannerPath.ToString();
            }


             var products=db.TblProducts
                            .Include(e => e.TblProductCategory)
                            .Where(e => e.IsActive == true && e.IsDelete == false && e.StoreId == StoreId)
                            .OrderByDescending(e=>e.CreatedDate)
                            .ToList();
            return products;
        }
        [Route("product-details/{title}")]
        public ActionResult ProductDetails(string title)
        {
            if (title == null || title=="" )
            {
                 return RedirectToAction("PageNotFound");
            }
            //----------------
            var storeId = 6;
            if (geturl() != "http://localhost:50279")
            {  
                var url2 = geturl();
                var StoreInformation = db.TblStoreInformations.Where(e => e.IsActive == true && e.IsDelete == false && e.StoreURL == url2).OrderByDescending(e => e.CreatedDate).FirstOrDefault();
               
                if (StoreInformation == null)
                {
                    return RedirectToAction("StoreNotAvailable");
                }
                storeId = StoreInformation.StoreId;
            }
            //  ----------------------

            
            var product = db.TblProducts.Where(e => e.SlugURL == title  && e.IsActive==true && e.IsDelete==false && e.StoreId== storeId).FirstOrDefault();
            if (product == null)
            {
                return RedirectToAction("PageNotFound");
            }
            product.Views = product.Views + 1;
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();
            CompleteProductInfo completeProductInfo = new CompleteProductInfo();
            completeProductInfo.TblProduct = product;
            completeProductInfo.tblProductAttributes = db.TblProductAttributes.Where(e => e.ProductID == product.ProductID).OrderByDescending(e=>e.CreatedDate).ToList();
            completeProductInfo.tblProductImages = db.TblProductImages.Where(e => e.ProductID == product.ProductID).ToList();
            return View(completeProductInfo);
        }
        public ActionResult Partial_productAttribute(int ProductId)
        {
            var productAttributes = db.TblProductAttributes.Where(e => e.ProductID == ProductId).Include(e => e.TblAttribute).ToList();
            return PartialView("Partial_productAttribute", productAttributes);
        }

        [Route("Cart")]
        public ActionResult Cart()
        {
            return View();
        }
        [HttpPost]
        [Route("Cart")]
        public ActionResult Cart(checkOutModel  checkOutModel)
        {
            try
            {
                string body = "";               
                var ToName = "";
                var ToEmail = "";
                var productName = (dynamic)null;
                var StoreName="";
                var Storelogo = "";
                var storeUrl = "";
                var j =Convert.ToInt16( checkOutModel.productCount) ;

                var str = geturl();
                var storeId = 0;

                if (str == "http://localhost:50279")
                {
                    storeId = 6;
                    var StoreInformation = db.TblStoreInformations.Where(e => e.StoreId == storeId).OrderByDescending(e => e.CreatedDate).FirstOrDefault();
                    storeId = StoreInformation.StoreId;
                    ToName = StoreInformation.Order_Notification_Name;
                    ToEmail = StoreInformation.Order_Notification_Email;
                    Storelogo = StoreInformation.Logo;
                    storeUrl = StoreInformation.StoreURL;
                    StoreName = StoreInformation.StoreName;
                }
                else
                {
                    var StoreInformation = db.TblStoreInformations.Where(e => e.IsActive == true && e.IsDelete == false && e.StoreURL == str).OrderByDescending(e => e.CreatedDate).FirstOrDefault();
                    storeId = StoreInformation.StoreId;
                    ToName = StoreInformation.Order_Notification_Name;
                    ToEmail = StoreInformation.Order_Notification_Email;
                    Storelogo = StoreInformation.Logo;
                    storeUrl = StoreInformation.StoreURL;
                    StoreName = StoreInformation.StoreName;
                }


                var productIds = checkOutModel.ProductId.Split(';');
                var productAttribute = checkOutModel.productAttribute.Split(';');
                var ProductPrice = checkOutModel.ProductPrice.Split(';');
                var productQuantity = checkOutModel.productQuantity.Split(';');

                var TotalSubPrice = checkOutModel.TotalSubPrice;
                TblOrderHistory tblOrderHistory;
                for (var i = 0; i < j; i++)
                {
                    tblOrderHistory = new TblOrderHistory();
                    var id= productIds[i].ToString();
                    tblOrderHistory.ProductID = Convert.ToInt16(id);
                    tblOrderHistory.ProductAtrributes = productAttribute[i].ToString();
                    tblOrderHistory.ProductPrice = ProductPrice[i].ToString();
                    tblOrderHistory.productQuantity = productQuantity[i].ToString();

                    tblOrderHistory.OrderStatusID = 3;
                    tblOrderHistory.TotalSubPrice = checkOutModel.TotalPrice;
                    tblOrderHistory.TotalPrice = checkOutModel.TotalPrice;
                    tblOrderHistory.comments = checkOutModel.comments;
                    tblOrderHistory.StoreId  = storeId;
                    tblOrderHistory.OrderByName = checkOutModel.EmployeeName;
                    tblOrderHistory.Phone = checkOutModel.Phone;
                    tblOrderHistory.Email = checkOutModel.Email;
                    tblOrderHistory.ShippingAddress = checkOutModel.ShippingAddress;
                    tblOrderHistory.country = checkOutModel.country;
                    tblOrderHistory.state = checkOutModel.state;
                    tblOrderHistory.postcode = checkOutModel.postcode;
                    tblOrderHistory.CreatedDate = DateTime.Now;
                    tblOrderHistory.ModifiedDate = DateTime.Now;
                    db.TblOrderHistories.Add(tblOrderHistory);
                    db.SaveChanges();    

                    productName = db.TblProducts.Find(tblOrderHistory.ProductID);
                    body = body+ "<tr style='background: #ffffff; color: #000; font-size:15px; text-align:left; font-family:Arial, Helvetica, sans-serif; vertical-align:top;'>" +
                                     "<td>" + tblOrderHistory.OrderID + " </td>" +
                                    "<td>" + productName.productTitle + " </td>" +
                                     "<td>" + tblOrderHistory.productQuantity + "</td>" +
                                      "<td>" + tblOrderHistory.ProductAtrributes + "</td>" +
                                   "</tr>";
                    

                }

                //----------- mail send to client
                StreamReader sr = new StreamReader(System.Web.Hosting.HostingEnvironment.MapPath("~/Content/mail.html"));
                string HTML_Body = sr.ReadToEnd();
                
                string final_Html_Body = HTML_Body.Replace("#name", ToName)
                                                      .Replace("#moreRows", body)
                                                      .Replace("#shippingAddress", checkOutModel.ShippingAddress)
                                                      .Replace("#CustomerName", checkOutModel.EmployeeName)
                                                      .Replace("#Phone", checkOutModel.Phone)
                                                      .Replace("#imgsrc", "http://crastores.com"+Storelogo)
                                                      .Replace("#StoreName", StoreName)
                                                      .Replace("#visitStore", storeUrl)
                                                      .Replace("#Comments", checkOutModel.comments)
                                                      .Replace("#Email", checkOutModel.Email);
                                                       
                sr.Close();

                string To = ToEmail;
                string mail_Subject = "New Order Notification -"+ StoreName ;
                var result = BaseUtil.sendEmailer(To, mail_Subject, final_Html_Body);
                //----------- mail send to costomer
                StreamReader sr1 = new StreamReader(System.Web.Hosting.HostingEnvironment.MapPath("~/Content/mailToCustomer.html"));
                string HTML_Body1 = sr1.ReadToEnd();

                string final_Html_Body1 = HTML_Body1.Replace("#name", checkOutModel.EmployeeName)
                                                      .Replace("#moreRows", body)
                                                      .Replace("#shippingAddress", checkOutModel.ShippingAddress)
                                                      .Replace("#CustomerName", checkOutModel.EmployeeName)
                                                      .Replace("#Phone", checkOutModel.Phone)
                                                      .Replace("#imgsrc", "http://crastores.com" + Storelogo)
                                                      .Replace("#StoreName", StoreName)
                                                      .Replace("#visitStore", storeUrl)
                                                      .Replace("#Comments", checkOutModel.comments)
                                                      .Replace("#Email", checkOutModel.Email);

                sr1.Close();

                string ToCustomerEmail = checkOutModel.Email;
                string ToCustomerSubject = "New Order Notification -" + StoreName;
                var result1 = BaseUtil.sendEmailer(ToCustomerEmail, ToCustomerSubject, final_Html_Body1);
                //----------- End mail send to costomer

              

                TempData["msg"] = "5";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["msg"] = "6";
               // TempData["msg"] = ex.Message;
            }
            return View();
        }
        public ActionResult PageNotFound()
        {
            Response.StatusCode = 404;
            return View();
        }
        public ActionResult BlankTempData()
        {
            TempData["msg"] = "";
            return View();
        }
        [Route("store-not-available")]
        public ActionResult StoreNotAvailable()
        {            
            return View();
        }

    }
}