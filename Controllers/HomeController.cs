using EthnoBot.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EthnoBot.Controllers
{
    public class HomeController : Controller
    {
        private EthnoBotEntities db = new EthnoBotEntities();
        public ActionResult Index()
        {
            countCartItems();
            if (
               User.IsInRole("Admin")
            ) return RedirectToAction("AdminIndex");
            else
            {

                return View();
            }
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

     
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AdminIndex()
        {
            ViewBag.Message = "This can be viewed only by users in Admin role only";
            return View();
        }
    }
}