using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
            List<string> categoryIds = new List<string>();
            for (int i = 0; i > categories.Count; i++)
            {
                categoryIds.Add(categories.ElementAt(i).CategoryId);
                Debug.Print("Added: " + categories.ElementAt(i).CategoryId + " to category ids!");
            }
           
            var products =db.Products.Where(x => x.Title.Contains(searchString) | x.Family.Contains(searchString) | categoryIds.Contains(x.CategoryId) | x.LatinName.Contains(searchString)).ToList();
            var Sellers = db.Sellers.Where(x => x.FirstName.Contains(searchString) ).ToList();
           
            SearchResultsViewModel model = new SearchResultsViewModel();
            model.products = (List<Product>)products;
            model.Sellers = (List<Seller>)Sellers;
            model.categories = (List<Category>)categories;
            return View(model);
        }
        public ActionResult ListProducts(string id)
        {
            var Products = db.Products.Where(x => x.CategoryId == id).ToList();
            ViewData["CategoryName"] = db.Categories.Where(x => x.CategoryId == id).ToList().First().Name;
            return View(Products);
        }
        public List<ListingViewModel> listingViewModels;
        public ActionResult ProductAndListings(string productId)
        {

            Product product = db.Products.FirstOrDefault(x=>x.ProductId==productId);
            List<Listing> listings = db.Listings.Where(x => x.ProductId == productId).ToList();
            Category category = db.Categories.FirstOrDefault(x => x.CategoryId == product.CategoryId);
            
           


           listingViewModels= new List<ListingViewModel>();
              

              
            for (int i = 0; i < listings.Count; i++)
            {
                ListingViewModel lo = new ListingViewModel();
                lo.Listing = listings.ElementAt(i);
             
                string sellerId =listings.ElementAt(i).SellerId;
                
                Seller seller = db.Sellers.Where(x => x.SellerId == sellerId).First();
              
             
                lo.UnitPriceKG =lo.Listing.UnitPriceKG;
                lo.UnitsKG = lo.Listing.UnitsKG;
                lo.Product = product;
                lo.Seller = seller;
                listingViewModels.Add(lo);
                
            }


            ProductAndListingsModel pm = new ProductAndListingsModel();
            pm.ListingViewModels = listingViewModels;
            pm.Product = product;
            pm.Category = category;
            return View(pm);
        }

        [Authorize]
        public ActionResult AddToBasket(string SellerId, string productId, string quantity, double unitPrice)
        {

            string userId = User.Identity.GetUserId();
            Guid userID = Guid.Parse(User.Identity.GetUserId());
            Cart cart = db.Carts.Where(x => x.UserId == userId).FirstOrDefault();
            CartItem  c = db.CartItems.Where(x => x.SellerId == SellerId && x.ProductId == productId).First();
            if (c == null)
            {
                c = new CartItem();
                c.SellerId = SellerId;
                c.ProductId = productId;
                c.UnitsKG = float.Parse(quantity);
                c.UnitPriceKG = float.Parse(unitPrice.ToString());
            }
            else { c.UnitsKG += float.Parse(quantity);c.UnitPriceKG = float.Parse(unitPrice.ToString()); }
        
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
                    Cart c = db.Carts.Where(x => x.UserId == userID).FirstOrDefault();
                    int count = db.CartItems.Where(x => x.CartId == c.CartId).ToList().Count;
                    Session["CartItemCount"] = count;
                }
            }
            catch (Exception e)
            {
                Exception ex = e;

            }
        }


        public ActionResult SellerAndListings(string sellerid)
        {
            try
            {
                Seller seller = db.Sellers.Where(x => x.SellerId == sellerid).First();
                List<Listing> listings = db.Listings.Where(x => x.SellerId == sellerid).ToList();




                List<ListingViewModel> listingViewModels = new List<ListingViewModel>();


                for (int i = 0; i < listings.Count; i++)
                {
                    ListingViewModel lo = new ListingViewModel();
                    lo.Listing = listings.ElementAt(i);

                    string productId = listings.ElementAt(i).ProductId;

                    Product product = db.Products.Where(x => x.ProductId == productId).First();
                    

                    lo.UnitPriceKG = lo.Listing.UnitPriceKG;
                    lo.UnitsKG = lo.Listing.UnitsKG;
                    lo.Product = product;
                    lo.Seller = seller;
                    listingViewModels.Add(lo);

                }


                SellerAndListingsModel pm = new SellerAndListingsModel();
                pm.listingViewModels = listingViewModels;
                pm.Seller = seller;

                return View(pm);
            }
            catch (Exception) { return View("Error"); }
        }



        public ActionResult SellerList()
        {
            var Sellers = db.Sellers.ToList();
                     
            return View(Sellers);
        }
        public string GetImage(string imageId)
        {
            ImageModel image = db.Images.Where(x => x.Name == imageId).First();
            byte[] imageBytes = image.Data;
            string base64String = "data:image/jpeg;base64," + Convert.ToBase64String(image.Data, 0, image.Data.Length);
            return base64String;
            //MemoryStream ms = new MemoryStream(imageBytes);
            // return File(ms, "image/jpeg", imageId);
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
