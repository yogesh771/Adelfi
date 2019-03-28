using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AdelfyStore.Models;

namespace AdelfyStore.Controllers
{
    [SessionTimeout]
    public class AttributeValuesController : Controller
    {
        private SBV_GroupStoreEntities db = new SBV_GroupStoreEntities();

        // GET: AttributeValues
        public ActionResult Index()
        {
            var tblAttributeValues = db.TblAttributeValues.Include(t => t.TblAttribute);
            return View(tblAttributeValues.ToList());
        }
      

        // GET: AttributeValues/Create
        public ActionResult Create()
        {
            ViewBag.AttributeId = db.TblAttributes
                                    .Where(e=>e.IsDelete==false)
                                    .Select(e => new { e.AttributeId, e.Attribute })
                                    .OrderBy(e => e.Attribute);            
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TblAttributeValue tblAttributeValue)
        {
            if (tblAttributeValue.orderNo == 0) { tblAttributeValue.orderNo = 1000; }
            if (ModelState.IsValid)
            {
                db.TblAttributeValues.Add(tblAttributeValue);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AttributeId = new SelectList(db.TblAttributes.Where(e => e.IsDelete == false), "AttributeId", "Attribute", tblAttributeValue.AttributeId);
            return View(tblAttributeValue);
        }

        // GET: AttributeValues/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TblAttributeValue tblAttributeValue = db.TblAttributeValues.Find(id);
            if (tblAttributeValue == null)
            {
                return HttpNotFound();
            }
            if (tblAttributeValue.orderNo == 1000) { tblAttributeValue.orderNo = 0; }
            ViewBag.AttributeId = new SelectList(db.TblAttributes.Where(e => e.IsDelete == false), "AttributeId", "Attribute", tblAttributeValue.AttributeId);
            return View(tblAttributeValue);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TblAttributeValue tblAttributeValue)
        {
            if (tblAttributeValue.orderNo == 0) { tblAttributeValue.orderNo = 1000; }
            if (ModelState.IsValid)
            {
                db.Entry(tblAttributeValue).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AttributeId = new SelectList(db.TblAttributes.Where(e => e.IsDelete == false), "AttributeId", "Attribute", tblAttributeValue.AttributeId);
            return View(tblAttributeValue);
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
