using EthnoBot.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace EthnoBot.Controllers
{
    public class AdminController : Controller
    {
        private EthnoBotEntities db = new EthnoBotEntities();
        // GET: Admin
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View();
        }



        //Product Details 
        [Authorize(Roles = "Admin")]
        public ActionResult ProductDetails(string id)
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
        [Authorize(Roles = "Admin")]
        public ActionResult ProductIndex()
        {  EthnoBotEntities db = new EthnoBotEntities();
            List<Product> model = db.Products.ToList();
            return View(model);
        }


        // GET: AdminProducts/Create
        [Authorize(Roles = "Admin")]
        public ActionResult CreateProduct()
        {
            ModifyProductViewModel m = new ModifyProductViewModel();


            return View(m);
        }


        // POST: AdminProducts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult EditProduct(ModifyProductViewModel m)
        {
            if (ModelState.IsValid)
            {

                db.Entry(m.Product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(m.Product);
        }

 
        // GET: AdminProducts/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult EditProduct(string id)
        {
           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Where(x => x.ProductId == id).First();
            if (product == null)
            {

                return HttpNotFound();
            }
            List<string> TagIds = new List<string>();
            List<TagAssociation> associations = db.TagAssociations.Where(x => x.ProductId == id).ToList();
            List<Tag> productTags = new List<Tag>();
            foreach (var x in associations)
            {
                string currentId = x.TagId;
                Tag currentTag = db.Tags.Where(y => y.TagId == currentId).First();
                productTags.Add(currentTag);
  //              TagIds.Add(x.TagId);
            }
               
            ModifyProductViewModel model = new ModifyProductViewModel();
            model.Product = product;
            model.CurrentProductTags = productTags;
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult PerformTagSearch(string SearchText)

        {
            Session["SearchString"] = SearchText;
            
            //used to store all unique product Ids, so no duplicates are shown
           
            //Final List of products to be shown in Search Results
        
           
          
                    List<Tag> tags = db.Tags.Where(x => x.Name.Contains(SearchText)||x.Description.Contains(SearchText)).ToList();
            List<TagSearchResultViewModel> results = new List<TagSearchResultViewModel>();
            foreach (var tag in tags)
            {
                TagSearchResultViewModel srvm = new TagSearchResultViewModel();
                srvm.Tag = tag;
                string categoryId = tag.TagCategoryId;
                TagCategory category= db.TagCategories.Where(x=>x.TagCategoryId==categoryId).First();
                srvm.TagCategory = category;
                results.Add(srvm);

            }

            return PartialView("SearchTagsPartial", results);
        }

    }
}