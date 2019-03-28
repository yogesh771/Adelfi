using AdelfyStore.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdelfyStore.Controllers
{
    public class JavaScriptController : Controller
    {
       readonly private SBV_GroupStoreEntities db = new SBV_GroupStoreEntities();
        // GET: JavaScript
        public bool RecordDelete(Int32 RecordId, string TblName)
        {
            bool flag = true;
            try
            {
                switch (TblName)
                {
                    case "TblBanners":
                        var banner = db.TblBanners.Find(RecordId);                       
                        banner.IsDelete = true;
                        db.Entry(banner).State = EntityState.Modified;
                        db.SaveChanges();
                        break;

                    case "TblCompanyType":
                        var CompanyType = db.TblCompanyTypes.Find(RecordId);
                        CompanyType.IsActive = false;
                        db.Entry(CompanyType).State = EntityState.Modified;
                        db.SaveChanges();
                        break;
                    case "TblStoreInformation":
                        var StoreInformations = db.TblStoreInformations.Find(RecordId);
                        StoreInformations.IsDelete = true;
                        db.Entry(StoreInformations).State = EntityState.Modified;
                        db.SaveChanges();
                        break;
                    case "TblProductCategories":
                        var ProductCategories = db.TblProductCategories.Find(RecordId);
                        ProductCategories.IsDelete = true;
                        db.Entry(ProductCategories).State = EntityState.Modified;
                        db.SaveChanges();
                        break;
                    case "TblSuppliers":
                        var TblSupplier = db.TblSuppliers.Find(RecordId);
                        TblSupplier.IsDelete = true;
                        db.Entry(TblSupplier).State = EntityState.Modified;
                        db.SaveChanges();
                        break;
                    case "TblOrderStatus":
                        var OrderStatus = db.TblOrderStatus.Find(RecordId);
                        OrderStatus.IsDelete = true;
                        db.Entry(OrderStatus).State = EntityState.Modified;
                        db.SaveChanges();
                        break;
                    case "TblAttributes":
                        var Attributes = db.TblAttributes.Find(RecordId);
                        Attributes.IsDelete = true;
                        db.Entry(Attributes).State = EntityState.Modified;
                        db.SaveChanges();
                        break;
                    case "TblProducts":
                        var Products = db.TblProducts.Find(RecordId);
                        Products.IsDelete = true;
                        db.Entry(Products).State = EntityState.Modified;                       
                        db.SaveChanges();
                        break;
                    case "TblProductsImages":
                        var ProductImages = db.TblProductImages.Find(RecordId);
                        BaseUtil.DeleteAlreadyExistingFile(ProductImages.ProductImageURL, "/Content/Images");
                        db.Entry(ProductImages).State = EntityState.Deleted;
                        db.SaveChanges();
                        break;
                    case "TblProductsAttributes":
                        var ProductAttributes = db.TblProductAttributes.Find(RecordId);
                        db.Entry(ProductAttributes).State = EntityState.Deleted;                       
                        db.SaveChanges();
                        break; 
                    case "TblAttributeValuess":
                        var AttributeValues = db.TblAttributeValues.Find(RecordId);
                        db.Entry(AttributeValues).State = EntityState.Deleted;
                        db.SaveChanges();
                        break;
                }
                
                

            }
            catch (Exception ex)
            {
                flag = false;
            }
            return flag;
        }


        
        public JsonResult IsSlugUrlExists(string value, int id)
        {
           
                bool found = db.TblAttributeValues.Any(e => e.value == value && e.AttributeId==id);
                if (!found)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                return Json(false, JsonRequestBehavior.AllowGet);
            
        }

        public JsonResult IsSupplierExists(string SupplierName, string Previoussuppliername)
        {
            if (SupplierName == Previoussuppliername)
                return Json(true, JsonRequestBehavior.AllowGet);
            else
            {
                bool found = db.TblSuppliers.Any(e => e.SupplierName == SupplierName);
                if (!found)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
    }
}