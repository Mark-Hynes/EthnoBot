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
    public class AdminProducerProductsController : Controller
    {
        private StoreEntities db = new StoreEntities();

        // GET: AdminProducerProducts
        public ActionResult Index()
        {
            return View(db.ProducerProducts.ToList());
        }

        // GET: AdminProducerProducts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProducerProduct producerProduct = db.ProducerProducts.Find(id);
            if (producerProduct == null)
            {
                return HttpNotFound();
            }
            return View(producerProduct);
        }

        // GET: AdminProducerProducts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminProducerProducts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProducerProductId,ProducerId,ProductId,Price")] ProducerProduct producerProduct)
        {
            if (ModelState.IsValid)
            {
                db.ProducerProducts.Add(producerProduct);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(producerProduct);
        }

        // GET: AdminProducerProducts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProducerProduct producerProduct = db.ProducerProducts.Find(id);
            if (producerProduct == null)
            {
                return HttpNotFound();
            }
            return View(producerProduct);
        }

        // POST: AdminProducerProducts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProducerProductId,ProducerId,ProductId,Price")] ProducerProduct producerProduct)
        {
            if (ModelState.IsValid)
            {
                db.Entry(producerProduct).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(producerProduct);
        }

        // GET: AdminProducerProducts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProducerProduct producerProduct = db.ProducerProducts.Find(id);
            if (producerProduct == null)
            {
                return HttpNotFound();
            }
            return View(producerProduct);
        }

        // POST: AdminProducerProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProducerProduct producerProduct = db.ProducerProducts.Find(id);
            db.ProducerProducts.Remove(producerProduct);
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
