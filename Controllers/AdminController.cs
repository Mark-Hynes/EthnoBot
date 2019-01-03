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

    }
}