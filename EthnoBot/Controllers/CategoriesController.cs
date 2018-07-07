using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EthnoBot.Models;
using Microsoft.AspNet.Identity;

namespace EthnoBot.Controllers
{
    public class CategoriesController : Controller
    {
        private EthnoBotEntities db = new EthnoBotEntities();
        
        // GET: Categories
        public ActionResult Index()
        {
            countCartItems();
            return View(db.Categories.ToList());
        }
     
        public ActionResult Search(String searchString)
        { ViewData["parameters"] = searchString;

            var categories = db.Categories.Where(x => x.Name.Contains(searchString)).ToList();
            List<int> categoryIds = new List<int>();
            for (int i = 0; i > categories.Count; i++)
            {
                categoryIds.Add(categories.ElementAt(i).CategoryId);
                Debug.Print("Added: " + categories.ElementAt(i).CategoryId + " to category ids!");
            }
           
            var products =db.Products.Where(x => x.Title.Contains(searchString) | x.Family.Contains(searchString) | categoryIds.Contains(x.CategoryId) | x.LatinName.Contains(searchString)).ToList();
            var producers = db.Producers.Where(x => x.Name.Contains(searchString) ).ToList();
           
            SearchResultsViewModel model = new SearchResultsViewModel();
            model.products = (List<Product>)products;
            model.producers = (List<Producer>)producers;
            model.categories = (List<Category>)categories;
            return View(model);
        }
        public ActionResult ListProducts(int id)
        {
            var Products = db.Products.Where(x => x.CategoryId == id).ToList();
            ViewData["CategoryName"] = db.Categories.Where(x => x.CategoryId == id).ToList().First().Name;
            return View(Products);
        }
    
        public ActionResult ProductAndListings(int id)
        {

            Product product = db.Products.FirstOrDefault(acc=>acc.ProductId==id);
            List<ProducerProduct> Listings = db.ProducerProducts.Where(x => x.ProductId == id).ToList();
            Category category = db.Categories.FirstOrDefault(x => x.CategoryId == product.CategoryId);
            
           


            List<ListingInfo> listings = new List<ListingInfo>();
              

              
            for (int i = 0; i < Listings.Count; i++)
            {
                ListingInfo lo = new ListingInfo();
                int prodID =Listings.ElementAt(i).ProducerId;
                
                var prods = db.Producers.Where(x => x.ProducerId == prodID).ToList();
                for (int j = 0; j < prods.Count; j++)
                {
                    Producer prod = prods.ElementAt(j);
                   
                    lo.Producer = prod ;
                    
                    
                }
             
                lo.Price = Listings.ElementAt(i).UnitPrice.ToString();
                lo.Product = product;
                listings.Add(lo);
                
            }


            ProductAndListingsModel pm = new ProductAndListingsModel();
            pm.listings = listings;
            pm.product = product;
            pm.category = category;
            return View(pm);
        }

        [Authorize]
        public ActionResult AddToBasket(string producerId,string productId, string quantity, int unitPrice)
        {
            int producer = Int32.Parse(producerId);
            int product = Int32.Parse(productId);
             int quantit = Int32.Parse(quantity);
            CartItem c = new CartItem();
            c.producer = db.Producers.Where(x => x.ProducerId == producer).FirstOrDefault();
            c.product = db.Products.Where(x => x.ProductId == product).FirstOrDefault();
            c.quantityKg = quantit;
            c.unitPrice = unitPrice;

            string userId = User.Identity.GetUserId();
            Guid userID = Guid.Parse(User.Identity.GetUserId());
            Cart cart = db.Carts.Where(x => x.UserID == userId).FirstOrDefault();
            cart.CartItems += "|ProducerId=" + producerId + "," + "ProductId=" + productId + "," + "UnitPrice=" + unitPrice + "," + "Quantity=" + quantity + "|";
          
            db.SaveChanges();
            countCartItems();


              return Redirect(Request.UrlReferrer.ToString());
        }

        public void countCartItems()
        {
            try
            {
                string userID = User.Identity.GetUserId();
                if (userID == null || userID.Equals(""))
                {
                    Session["CartItemCount"] = 0;
                }
                else
                {
                    Cart c = db.Carts.Where(x => x.UserID == userID).FirstOrDefault();
                    string[] cartCount = c.CartItems.Split('|');
                    int realCount = 0;
                    for (int i = 0; i < cartCount.Length; i++)
                    {
                        if (!cartCount[i].Equals(""))
                        { realCount++; }
                    }
                    Session["CartItemCount"] = realCount;
                }
            }
            catch (Exception e)
            {
                Exception ex = e;

            }
        }


        public ActionResult ProducerAndListings(int id)
        {
            Producer producer = db.Producers.FirstOrDefault(acc => acc.ProducerId == id);
            List<ProducerProduct> Listings = db.ProducerProducts.Where(x => x.ProducerId == id).ToList();
  



            List<ListingInfo> listings = new List<ListingInfo>();



            for (int i = 0; i < Listings.Count; i++)
            {
                ListingInfo lo = new ListingInfo();
                int prodID = Listings.ElementAt(i).ProductId;

                var prods = db.Products.Where(x => x.ProductId == prodID).ToList();
                for (int j = 0; j < prods.Count; j++)
                {
                    Product prod = prods.ElementAt(j);

                    lo.Product = prod;


                }

                lo.Price = Listings.ElementAt(i).UnitPrice.ToString();
                lo.Producer = producer;
                listings.Add(lo);

            }


            ProducerAndListingsModel pm = new ProducerAndListingsModel();
            pm.listings = listings;
            pm.producer = producer;
           
            return View(pm);
        }



        public ActionResult ProducerList()
        {

            return View(db.Producers.ToList());
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
