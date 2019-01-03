using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EthnoBot.Models;

namespace EthnoBot.Controllers
{
    public class AdminProductsController : Controller
    {
        private EthnoBotEntities db = new EthnoBotEntities();

        // GET: AdminProducts
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View(db.Products.ToList());
        }

        // GET: AdminProducts/Details/5
       

      
        // POST: AdminProducts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(ModifyProductViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {
                Guid guid = Guid.NewGuid();
                productViewModel.Product.ProductId = guid.ToString();
                db.Products.Add(productViewModel.Product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(productViewModel);
        }

     
       
        // GET: AdminProducts/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: AdminProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
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
