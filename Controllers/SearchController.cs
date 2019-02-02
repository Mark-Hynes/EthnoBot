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
    public class SearchController : Controller
    {
        private EthnoBotEntities db = new EthnoBotEntities();

        // GET: Categories
        public ActionResult Index()
        {
            ProductSearchViewModel psvm = new ProductSearchViewModel();
            psvm.readTagCategories(db);
            psvm.readAndSortTags(db);
            psvm.FeatureSearchItems = db.FeatureSearchItems.ToList();
            CountCartItems();
            return View(psvm);
        }

        public ActionResult SearchWString(string searchString)
        {
            Session["SearchString"] = searchString;
            return RedirectToAction("Index");
        }

        public ActionResult PerformSearch(string SearchText, string SelectedTags)

        {
            if (SearchText.Equals(""))
            {

            }
            Session["SearchString"] = SearchText;
            string[] selectedTags = SelectedTags.Split(',');
            //used to store all unique product Ids, so no duplicates are shown
            List<string> productIds = new List<string>();
            //Final List of products to be shown in Search Results
            List<Product> finalResults = new List<Product>();
            //List of tags that contained/matched the search query
            //List<Tag> tagResults = db.Tags.Where(x => x.Name.Contains(SearchText)).ToList();
            //List of products that contained/matched the search query
            //   List<Product> productResults = db.Products.Where(x => x.Title.Contains(SearchText)).ToList();
            //  foreach (Product p in productResults)
            //  {
            //      if (!productIds.Contains(p.ProductId))
            //      {   //adding all unique product ids to the list
            //          productIds.Add(p.ProductId);


            //       }
            //   }
            //adding the product results to finalResults
            //   finalResults = productResults;
            //go through the returned tags, and search tagAssociations table
            //to find any additional products not already in the Product list
            for (int i = 0; i < selectedTags.Length; i++)
            {
                string tagId = selectedTags[i];
                try
                {
                    List<TagAssociation> tagAssociations = db.TagAssociations.Where(x => x.TagId == tagId).ToList();
                    foreach (TagAssociation ta in tagAssociations)
                    {
                        if (!productIds.Contains(ta.ProductId.Replace("\\r\\n", "")))
                        {
                            string searchtext = SearchText.ToLower();
                            Product productFromTag = db.Products.Where(x => x.ProductId.Trim().Contains(ta.ProductId.Trim())).First();

                            if (SearchText.Equals(""))
                            {
                                productIds.Add(ta.ProductId.Replace("\\r\\n", ""));
                                //add new product to the final list
                                finalResults.Add(productFromTag);
                            }
                           else if ((productFromTag.Title.IndexOf(SearchText, 0, StringComparison.CurrentCultureIgnoreCase) != -1)||(productFromTag.Abstract.IndexOf(SearchText, 0, StringComparison.CurrentCultureIgnoreCase) != -1)||(productFromTag.Description.IndexOf(SearchText, 0, StringComparison.CurrentCultureIgnoreCase) != -1))
                            {
                                productIds.Add(ta.ProductId.Replace("\\r\\n", ""));
                                //add new product to the final list
                                finalResults.Add(productFromTag);
                            } else {
                                Console.WriteLine(productFromTag.Title + " did not contain " + SearchText);
                            } }
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                    //No Tag association was found for a product that was found earlier through the initial product search
                }
            }
            SearchResultsViewModel srvm = new SearchResultsViewModel();
            srvm.products = finalResults;
            return PartialView("SearchPartial", srvm);
        }



        public ActionResult ListProducts(string id)
        {

            return View();
        }
        public List<ListingViewModel> listingViewModels;
        public ActionResult ProductAndListings(string id)
        {

            Product product = db.Products.FirstOrDefault(x => x.ProductId ==id);
            List<Listing> listings = db.Listings.Where(x => x.ProductId == id).ToList();





            listingViewModels = new List<ListingViewModel>();



            for (int i = 0; i < listings.Count; i++)
            {
                ListingViewModel lo = new ListingViewModel();
                lo.Listing = listings.ElementAt(i);
                string listingId = lo.Listing.ListingId;
                string sellerId = listings.ElementAt(i).SellerId;
                lo.Offers = db.Offers.Where(x => x.ListingId == lo.Listing.ListingId).ToList();
                Seller seller = db.Sellers.Where(x => x.SellerId == sellerId).First();
                               
               
                lo.Product = product;
                lo.Seller = seller;
                //lo.ProcessingOption = db
                listingViewModels.Add(lo);

            }

            List<string> tags =new List<string>();
            ProductAndListingsModel pm = new ProductAndListingsModel();pm.ProductTags = new List<Tag>();
            pm.ListingViewModels = listingViewModels;
            pm.TagCategories = db.TagCategories.ToList();
            string productId = product.ProductId;
            List<TagAssociation> associations = db.TagAssociations.Where(x => x.ProductId.Contains(productId)).ToList();
            foreach (var item in associations)
            {
                if (!tags.Contains(item.TagId)) { tags.Add(item.TagId);
                    string tagId = item.TagId;
                    Tag t = db.Tags.Where(x => x.TagId.Contains(tagId)).First();
                    pm.ProductTags.Add(t); }
            }
            
            ListingTagCategory category = db.ListingTagCategories.Where(x => x.Name.Contains("Processing Options")).FirstOrDefault();
            string processingOptionsTagCategory = category.ListingTagCategoryId.Trim();
            pm.processingOptions = db.ListingTags.Where(x =>x.ListingTagCategoryId.Contains(processingOptionsTagCategory)).ToList(); 
            pm.Product = product;

            return View(pm);
        }

        [Authorize]
        public ActionResult AddToBasket(string listingId, string quantity)
        {

            string userId = User.Identity.GetUserId();
            Guid cartitemId = Guid.NewGuid();
            Listing l = db.Listings.Where(x => x.ListingId == listingId).First();
            Cart cart = db.Carts.Where(x => x.UserId == userId).FirstOrDefault();

            CartItem c;
            try
            {
                c = db.CartItems.Where(x => x.ListingId == l.ListingId).First();
                c.UnitsKG += Convert.ToDecimal(quantity);
                db.SaveChanges();
                CountCartItems();
                return Redirect(Request.UrlReferrer.ToString());

            }
            catch (Exception e)
            {
                c = new CartItem();
                c.CartItemId = cartitemId.ToString();
                c.CartId = cart.CartId;
                c.ListingId = l.ListingId;
                c.UnitsKG = Convert.ToDecimal(quantity);
                db.CartItems.Add(c);
                db.SaveChanges();
                CountCartItems();
                return Redirect(Request.UrlReferrer.ToString());
            }

        }

        public void CountCartItems()
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


        public ActionResult SellerAndListings(string sellerId)
        {
            try
            {
                Seller seller = db.Sellers.Where(x => x.SellerId == sellerId).First();
                List<Listing> listings = db.Listings.Where(x => x.SellerId == sellerId).ToList();




                List<ListingViewModel> listingViewModels = new List<ListingViewModel>();


                for (int i = 0; i < listings.Count; i++)
                {
                    ListingViewModel lo = new ListingViewModel();
                    lo.Listing = listings.ElementAt(i);

                    string productId = listings.ElementAt(i).ProductId;

                    Product product = db.Products.Where(x => x.ProductId == productId).First();


                   
                    lo.Product = product;
                    lo.Seller = seller;
                    listingViewModels.Add(lo);

                }


                SellerAndListingsModel pm = new SellerAndListingsModel();
                pm.listingViewModels = listingViewModels;
                pm.Seller = seller;

                return View(pm);
            }
            catch (Exception err)
            {
                string error = err.InnerException.ToString();
                return View("Error");
            }
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


    }
}