namespace Comparator.Controllers
{
    using Comparator.Data;
    using Comparator.Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    public class PDFController : Controller
    {
        private PDFContext db = new PDFContext();
        // GET: PDF
        public ActionResult Index()
        {
            return View(db.Files.ToList());
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HttpPostedFileBase upload)
        {
            if (upload != null)
            {
                string filename = System.IO.Path.GetFileName(upload.FileName);
                upload.SaveAs(Server.MapPath("~/Files/" + filename));
            }
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        /*https://metanit.com/sharp/articles/26.php*/
        public ActionResult Delete()
        {
            return View(db.Files.ToList());
        }
        [HttpPost]
        public ActionResult Delete(IEnumerable<int> PDFID)
        {
            foreach (var id in PDFID)
            {
                var file = db.Files.Single(p => p.PDFID == id);
                db.Files.Remove(file);
            }

            db.SaveChanges();
            return RedirectToAction("Delete");
        }
    }
}