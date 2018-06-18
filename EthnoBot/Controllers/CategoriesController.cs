﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EthnoBot.Models;

namespace EthnoBot.Controllers
{
    public class CategoriesController : Controller
    {
        private StoreEntities db = new StoreEntities();

        // GET: Categories
        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }

        public ActionResult ListProducts(int id)
        {
            var Products = db.Products.Where(x => x.CategoryId == id).ToList();
            return View(Products);
        }

        public ActionResult ProductAndListings(int id)
        {
            Product product = db.Products.FirstOrDefault(acc=>acc.ProductId==id);
            var Listings = db.ProducerProducts.Where(x => x.ProductId == id).ToList();
            var model = new ProductAndListingsModel { product = product, producerProducts = Listings };
            ViewData["ProdObj"] = product;


            List<ListingInfo> listings = new List<ListingInfo>();
                List<ProducerProduct> listOfListings=null;

                if (Listings != null)
                {
                    listOfListings = (List<ProducerProduct>) Listings;
                }
            for (int i = 0; i < listOfListings.Count; i++)
            {
                ListingInfo lo = new ListingInfo();
                int prodID = listOfListings.ElementAt(i).ProducerId;
                
                var prods = db.Producers.Where(x => x.ProducerId == prodID).ToList();
                for (int j = 0; j < prods.Count; j++)
                {
                    Producer prod = prods.ElementAt(j);
                    Debug.WriteLine(prod.ProducerId);
                    lo.ProducerName = prod.Name;
                    lo.ProducerMobile = prod.Mobile;
                    lo.ProducerAddress = prod.Address;
                    Debug.WriteLine("Name: " + prod.Name);
                    Debug.WriteLine("Mobile: " + prod.Mobile);
                    Debug.WriteLine("Address: " + prod.Address);
                }
                Debug.WriteLine("Cost: " + listOfListings.ElementAt(i).Price);
                lo.Price = listOfListings.ElementAt(i).Price.ToString();
                listings.Add(lo);
                
            }
            ViewData["Listings"] = listings;

            return View(Listings);
        }

        // GET: Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // GET: Categories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
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
