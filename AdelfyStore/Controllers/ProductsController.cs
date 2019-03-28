using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AdelfyStore.Models;
using Newtonsoft.Json;

namespace AdelfyStore.Controllers
{
    [SessionTimeout]
    public class ProductsController : Controller
    {
        private SBV_GroupStoreEntities db = new SBV_GroupStoreEntities();

        // GET: Products
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {               
                var tblProducts = db.sp_productDetails().Where(e => e.StoreId == id).ToList();                                               
                 BaseUtil.SetSessionValue(AdminInfo.StoreID.ToString(), id.ToString());                      
                return View(tblProducts);
            }
        }
        public ActionResult products()
        {           
            var tblProducts = db.sp_productDetails().ToList();
            return View(tblProducts);
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TblProduct tblProduct = db.TblProducts.Find(id);
            if (tblProduct == null)
            {
                return HttpNotFound();
            }
            IEnumerable<TblProductAttribute> tblProductAttributes = db.TblProductAttributes.Where(e => e.ProductID == id).Include(e => e.TblAttribute).ToList();
            IEnumerable<TblProductImage> tblProductImage = db.TblProductImages.Where(e => e.ProductID == id).ToList();
            CompleteProductInfo completeProductInfo = new CompleteProductInfo();
            completeProductInfo.TblProduct = tblProduct;
            completeProductInfo.tblProductAttributes = tblProductAttributes;
            completeProductInfo.tblProductImages = tblProductImage;
            return View(completeProductInfo);
        }
        public ActionResult NewProduct()
        {
            ViewBag.StoreId = db.TblStoreInformations.Where(e => e.IsDelete == false).Select(e => new { e.StoreId, e.StoreName }).OrderBy(e => e.StoreName);
            ViewBag.ProductCategoryId = db.TblProductCategories.Where(e => e.IsDelete == false).Select(e => new { e.ProductCategoryId, e.CategoryName }).OrderBy(e => e.CategoryName);
            ViewBag.SupplierId = db.TblSuppliers.Where(e => e.IsDelete == false && e.IsActive == true).Select(e => new { e.SupplierId, e.SupplierName }).OrderBy(e => e.SupplierName);

            ViewBag.Attributes = db.TblAttributes.Where(e => e.IsDelete == false);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewProduct(TblProduct tblProduct, IEnumerable<HttpPostedFileBase> file)
        {
            try
            {
                ViewBag.Attributes = db.TblAttributes.Where(e => e.IsDelete == false);
                ViewBag.StoreId = new SelectList(db.TblStoreInformations.Where(e => e.IsDelete == false), "StoreId", "StoreName", tblProduct.StoreId);
                ViewBag.ProductCategoryId = new SelectList(db.TblProductCategories.Where(e => e.IsDelete == false), "ProductCategoryId", "CategoryName", tblProduct.ProductCategoryId);
                ViewBag.SupplierId = new SelectList(db.TblSuppliers.Where(e => e.IsDelete == false && e.IsActive == true).Select(e => new { e.SupplierId, e.SupplierName }), "SupplierId", "SupplierName", tblProduct.SupplierId);
                //tblProduct = productModel.Product;
                tblProduct.IsDelete = false;
                tblProduct.IsActive = true;
                tblProduct.CreatedBy = Convert.ToInt32(BaseUtil.GetSessionValue(AdminInfo.LoginID.ToString()));
                tblProduct.ModifiedBy = Convert.ToInt32(BaseUtil.GetSessionValue(AdminInfo.LoginID.ToString()));
                tblProduct.CreatedDate = DateTime.Now;
                tblProduct.ModifiedDate = DateTime.Now;               
                tblProduct.PriceDispay = true;
                tblProduct.ProductName = tblProduct.productTitle;
                if (tblProduct.srNumber == 0) { tblProduct.srNumber = 1000; }
                List<TblProductImage> productImageList = new List<TblProductImage>();
                if (file != null)
                {
                    var i = 1;

                    TblProductImage productImage;
                    foreach (var p in file)
                    {
                        if (p != null)
                        {                            
                            productImage = new TblProductImage();
                            productImage.ProductImageURL = BaseUtil.UploadFile(p, "/Content/ProductImages");
                            productImageList.Add(productImage);
                            i++;
                        }
                    }
                }
                else
                {
                    tblProduct.DisplayImg = "/images/item-01.jpgg";
                }

                if (ModelState.IsValid)
                {
                    db.TblProducts.Add(tblProduct);
                    db.SaveChanges();
                    foreach (var productImage in productImageList)
                    {
                        productImage.ProductID = tblProduct.ProductID;
                    }
                    db.TblProductImages.AddRange(productImageList);
                    db.SaveChanges();
                    List<TblProductAttribute> LsttblProductAttributes = new List<TblProductAttribute>();
                    TblProductAttribute tblProductAttribute;
                    if (tblProduct.Attributes != null && tblProduct.Attributes != "")
                    {
                        var arr = tblProduct.Attributes.Split('$');
                        for (var i = 0; i < arr.Length - 1; i++)
                        {
                            var att = arr[i].Split('!');

                            var dd = att[1].Split(';');
                            for (var k = 0; k < dd.Length - 1; k++)
                            {
                                tblProductAttribute = new TblProductAttribute();
                                tblProductAttribute.AttributeId = Convert.ToInt16(att[0]);
                                tblProductAttribute.MeasurementUnits = dd[k];
                                tblProductAttribute.ProductID = tblProduct.ProductID;
                                tblProductAttribute.CreatedDate = DateTime.Now;
                                LsttblProductAttributes.Add(tblProductAttribute);
                            }
                        }
                    }
                    db.TblProductAttributes.AddRange(LsttblProductAttributes);
                    db.SaveChanges();
                    TempData["msg"] = "1";
                    return RedirectToAction("products");
                }
                TempData["msg"] = "0";
            }
            catch (Exception e)
            {
                TempData["msg"] = "0";
            }
            return View(tblProduct);
        }
        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.ProductCategoryId = db.TblProductCategories.Where(e => e.IsDelete == false).Select(e => new { e.ProductCategoryId, e.CategoryName }).OrderBy(e => e.CategoryName);
            ViewBag.SupplierId = db.TblSuppliers.Where(e => e.IsDelete == false && e.IsActive == true).Select(e => new { e.SupplierId, e.SupplierName }).OrderBy(e => e.SupplierName);

            ViewBag.Attributes = db.TblAttributes.Where(e => e.IsDelete == false);
            return View();
        }

        public JsonResult IsSlugUrlExists(string SlugURL, string PreviousSlugURL, int StoreId)
        {
            if (SlugURL == PreviousSlugURL)
                return Json(true, JsonRequestBehavior.AllowGet);
            else
            {              
                bool found = db.TblProducts.Any(e => e.SlugURL == SlugURL && e.StoreId== StoreId && e.IsDelete==false);
                if (!found)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TblProduct tblProduct, IEnumerable<HttpPostedFileBase> file)
        {
            try
            {
                ViewBag.Attributes = db.TblAttributes.Where(e => e.IsDelete == false);
                ViewBag.ProductCategoryId = new SelectList(db.TblProductCategories.Where(e => e.IsDelete == false), "ProductCategoryId", "CategoryName", tblProduct.ProductCategoryId);
                ViewBag.SupplierId = new SelectList(db.TblSuppliers.Where(e => e.IsDelete == false && e.IsActive == true).Select(e => new { e.SupplierId, e.SupplierName }), "SupplierId", "SupplierName", tblProduct.SupplierId);
                //tblProduct = productModel.Product;
                tblProduct.IsDelete = false;
                tblProduct.IsActive = true;
                tblProduct.CreatedBy = Convert.ToInt32(BaseUtil.GetSessionValue(AdminInfo.LoginID.ToString()));
                tblProduct.ModifiedBy = Convert.ToInt32(BaseUtil.GetSessionValue(AdminInfo.LoginID.ToString()));
                tblProduct.CreatedDate = DateTime.Now;
                tblProduct.ModifiedDate = DateTime.Now;
                tblProduct.StoreId = Convert.ToInt32(BaseUtil.GetSessionValue(AdminInfo.StoreID.ToString()));
                tblProduct.PriceDispay = true;
                tblProduct.ProductName = tblProduct.productTitle;
                if (tblProduct.srNumber == 0) { tblProduct.srNumber = 1000; }
                List<TblProductImage> productImageList = new List<TblProductImage>();
                if (file != null)
                {
                    var i = 1;

                    TblProductImage productImage;
                    foreach (var p in file)
                    {
                        if (p != null)
                        {                           
                            productImage = new TblProductImage();
                            productImage.ProductImageURL = BaseUtil.UploadFile(p, "/Content/ProductImages");
                            productImageList.Add(productImage);
                            i++;
                        }
                    }
                }
                else
                {
                    tblProduct.DisplayImg = "/images/item-01.jpgg";
                }

                if (ModelState.IsValid)
                {
                    db.TblProducts.Add(tblProduct);
                    db.SaveChanges();
                    foreach (var productImage in productImageList)
                    {
                        productImage.ProductID = tblProduct.ProductID;
                    }
                    db.TblProductImages.AddRange(productImageList);
                    db.SaveChanges();
                    List<TblProductAttribute> LsttblProductAttributes = new List<TblProductAttribute>();
                    TblProductAttribute tblProductAttribute;
                    if (tblProduct.Attributes != null && tblProduct.Attributes != "")
                    {
                        var arr = tblProduct.Attributes.Split('$');
                        for (var i = 0; i < arr.Length - 1; i++)
                        {
                            var att = arr[i].Split('!');
                            if (att[1] != "")
                            {
                                var dd = att[1].Split(',').ToArray();
                                for (var k = 0; k < dd.Length - 1; k++)
                                {
                                    tblProductAttribute = new TblProductAttribute();
                                    tblProductAttribute.AttributeId = Convert.ToInt16(att[0]);
                                    tblProductAttribute.MeasurementUnits = dd[k];
                                    tblProductAttribute.ProductID = tblProduct.ProductID;
                                    tblProductAttribute.CreatedDate = DateTime.Now;
                                    LsttblProductAttributes.Add(tblProductAttribute);
                                }
                            }
                        }
                    }
                    db.TblProductAttributes.AddRange(LsttblProductAttributes);
                    db.SaveChanges();
                    TempData["msg"] = "1";
                    return RedirectToAction("Index", new { id = tblProduct.StoreId });
                }
                TempData["msg"] = "0";
            }
            catch (Exception e)
            {
                TempData["msg"] = "0";
            }
            return View(tblProduct);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TblProduct tblProduct = db.TblProducts.Find(id);
            if (tblProduct == null)
            {
                return HttpNotFound();
            }
            if (tblProduct.srNumber == 1000)
            {
                tblProduct.srNumber = 0;

            }
            BaseUtil.SetSessionValue(AdminInfo.StoreID.ToString(), tblProduct.StoreId.ToString());
            ViewBag.ProductCategoryId = new SelectList(db.TblProductCategories.Where(e => e.IsDelete == false).Select(e => new { e.ProductCategoryId, e.CategoryName }), "ProductCategoryId", "CategoryName", tblProduct.ProductCategoryId);
            ViewBag.SupplierId = new SelectList(db.TblSuppliers.Where(e => e.IsDelete == false && e.IsActive == true).Select(e => new { e.SupplierId, e.SupplierName }), "SupplierId", "SupplierName", tblProduct.SupplierId);

            return View(tblProduct);
        }


        [HttpPost]
       
        public ActionResult Edit(TblProduct tblProduct)
        {
            try
            {
                ViewBag.ProductCategoryId = new SelectList(db.TblProductCategories.Where(e => e.IsDelete == false).Select(e => new { e.ProductCategoryId, e.CategoryName }), "ProductCategoryId", "CategoryName", tblProduct.ProductCategoryId);
                ViewBag.SupplierId = new SelectList(db.TblSuppliers.Where(e => e.IsDelete == false && e.IsActive == true).Select(e => new { e.SupplierId, e.SupplierName }), "SupplierId", "SupplierName", tblProduct.SupplierId);
                tblProduct.ModifiedBy = Convert.ToInt32(BaseUtil.GetSessionValue(AdminInfo.LoginID.ToString()));
                tblProduct.ModifiedDate = DateTime.Now;
                tblProduct.ProductName = tblProduct.productTitle;
                if (tblProduct.srNumber == 0) { tblProduct.srNumber = 1000; }
                if (ModelState.IsValid)
                {
                    db.Entry(tblProduct).State = EntityState.Modified;
                    db.SaveChanges();
                    var AlreadyselectedAttributes = db.TblProductAttributes.Where(e => e.ProductID == tblProduct.ProductID).ToList();
                    db.TblProductAttributes.RemoveRange(AlreadyselectedAttributes);
                    db.SaveChanges();
                    List<TblProductAttribute> LsttblProductAttributes = new List<TblProductAttribute>();
                    TblProductAttribute tblProductAttribute;
                    if (tblProduct.Attributes != null && tblProduct.Attributes != "")
                    {
                        var arr = tblProduct.Attributes.Split('$');
                        for (var i = 0; i < arr.Length - 1; i++)
                        {
                            var att = arr[i].Split('!');
                            if (att[1] != "")
                            {
                                var dd = att[1].Split(',').ToArray();
                                for (var k = 0; k < dd.Length - 1; k++)
                                {
                                    tblProductAttribute = new TblProductAttribute();
                                    tblProductAttribute.AttributeId = Convert.ToInt16(att[0]);
                                    tblProductAttribute.MeasurementUnits = dd[k];
                                    tblProductAttribute.ProductID = tblProduct.ProductID;
                                    tblProductAttribute.CreatedDate = DateTime.Now;
                                    LsttblProductAttributes.Add(tblProductAttribute);
                                }
                            }
                        }
                    }
                    db.TblProductAttributes.AddRange(LsttblProductAttributes);
                    db.SaveChanges();


                    TempData["msg"] = "2";
                    return RedirectToAction("Index", new { id = tblProduct.StoreId });
                }
            }
            catch (Exception e)
            {
                TempData["msg"] = "0";
            }
            return View(tblProduct);
        }

        public ActionResult Partial_productAttribute(int ProductId)
        {
            var productAttributes = db.TblProductAttributes.Where(e => e.ProductID == ProductId).Include(e => e.TblAttribute).OrderByDescending(e=>e.CreatedDate).ToList();
            return PartialView("Partial_productAttribute", productAttributes);
        }

        public ActionResult Partial_productAttributeupdated()
        {
            var productAttributes = db.TblAttributes.Where(e=>e.IsDelete==false).Include(e => e.TblProductAttributes).ToList();
           
            return PartialView("Partial_productAttributeupdated", productAttributes);
        }
        public ActionResult Partial_productAttributeEdit(int id)
        {
            var productAttributes = db.TblAttributes.Where(e => e.IsDelete == false).ToList();
            ViewBag.SelectedValues = db.TblProductAttributes.Where(e=>e.ProductID== id).ToList();
           
            return PartialView("Partial_productAttributeEdit", productAttributes);
        }
        public ActionResult TopViewed(int id)
        {
            var TopViewedProducts = db.sp_TpViewedproducts(id).ToList();
            return View(TopViewedProducts);
        }
        public ActionResult BestSellers(int id)
        {
            var sp_TopSellers = db.sp_TopSellers(id).ToList();
            return View(sp_TopSellers);
        }
        public ActionResult CustomersAccounts(int id)
        {
            var sp_Accounts = db.sp_Accounts(id).ToList();
            return View(sp_Accounts);
        }
        [HttpPost]
        public int Upload(FormCollection formCollection)
        {
            var filePath = ""; var v = 0;

            if (formCollection["productId"] != null)
            {
                v = Convert.ToInt16(formCollection["productId"]);
            }
            if (Request.Files.Count > 0)
            {
                var i = 1;
                foreach (string file in Request.Files)
                {
                    var _file = Request.Files[file];                                                       
                    filePath = BaseUtil.UploadFile(_file, "/Content/Images");
                    TblProductImage tblProductImage = new TblProductImage();
                    tblProductImage.ProductID = v;
                    tblProductImage.ProductImageURL = filePath;
                    db.TblProductImages.Add(tblProductImage);
                }
                db.SaveChanges();
            }
            return 1;
        }
        public int AddProductAttributes(int productId, string attribute, int AttributeId)
        {
            try
            {
                TblProductAttribute tblProductAttribute = new TblProductAttribute();

                tblProductAttribute.AttributeId = AttributeId;
                tblProductAttribute.MeasurementUnits = attribute;
                tblProductAttribute.ProductID = productId;
                tblProductAttribute.CreatedDate = DateTime.Now;

                db.TblProductAttributes.Add(tblProductAttribute);
                db.SaveChanges();
                return 1;
            }
            catch (Exception e)
            {

            }
            return 0;
        }

        public int manageImageOrder(string arr)
        {
            try
            {
                dynamic func_param = JsonConvert.DeserializeObject(arr);
                var count = func_param.Count;

                for (var i = 0; i < count; i++)
                {
                    var imageid_orderid = func_param[i];
                    var imageid = Convert.ToInt32((imageid_orderid.ToString().Split('-'))[0]);
                    var orderid = (imageid_orderid.ToString().Split('-'))[1];
                    var ProductImag = db.TblProductImages.Find(imageid);
                    if (orderid != "")
                    {
                        ProductImag.orderNo = Convert.ToInt32(orderid);
                    }
                    else
                    {
                        ProductImag.orderNo = null;
                    }
                    db.Entry(ProductImag).State = EntityState.Modified;

                }
                db.SaveChanges();
                return 1;
            }
            catch (Exception e)
            {
                
            }
            return 1;
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
