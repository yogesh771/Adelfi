using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AdelfyStore.Models;

namespace AdelfyStore.Controllers
{
    public class ThemesController : Controller
    {
        private SBV_GroupStoreEntities db = new SBV_GroupStoreEntities();

        // GET: Themes
        public ActionResult Index()
        {
            return View(db.TblThemes.ToList());
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
