using EthnoBot.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EthnoBot.Controllers
{

    public class CartController : Controller
    {
        private EthnoBotEntities db = new EthnoBotEntities();
        // GET: Cart
        [Authorize]
        public ActionResult Index()
        {
           List<CartItem> items = new List<CartItem>();
            string userID = User.Identity.GetUserId();
            Cart c = db.Carts.Where(x => x.UserID == userID).FirstOrDefault();

            string[] cartItemsAsString = c.CartItems.Split('|');

            for (int i = 0; i < cartItemsAsString.Length; i++)
            {
                if (cartItemsAsString[i].Equals(""))
                { continue; }

                CartItem item = new CartItem();
                string[] individualCartItemString = cartItemsAsString[i].Split(',');
               
                int producerid = Int32.Parse(individualCartItemString[0].Replace("ProducerId=",""));
                int productid = Int32.Parse(individualCartItemString[1].Replace("ProductId=", ""));

                item.producer = db.Producers.Where(x => x.ProducerId == producerid ).FirstOrDefault();
                item.product = db.Products.Where(x => x.ProductId == productid).FirstOrDefault();
                item.unitPrice = Int32.Parse(individualCartItemString[2].Replace("UnitPrice=", ""));
                item.quantityKg = Int32.Parse(individualCartItemString[3].Replace("Quantity=",""));
                item.total = item.unitPrice * item.quantityKg;
                items.Add(item);
            }
            countCartItems();
            return View(items);
        }

        [Authorize]
        public ActionResult RemoveFromBasket(string producerId, string productId, string quantity, string unitPrice)
        {
            string userID = User.Identity.GetUserId();
            Cart c = db.Carts.Where(x => x.UserID == userID).FirstOrDefault();

            string[] cartItemsAsString = c.CartItems.Split('|');

            for (int i = 0; i < cartItemsAsString.Length; i++)
            {
                if (cartItemsAsString[i].Equals(""))
                { continue; }

                CartItem item = new CartItem();
                string[] individualCartItemString = cartItemsAsString[i].Split(',');

                string producerid1 = individualCartItemString[0].Replace("ProducerId=", "");
               string productid1 = individualCartItemString[1].Replace("ProductId=", "");


               
                string unitPrice1 = individualCartItemString[2].Replace("UnitPrice=", "");
                string quantityKg = individualCartItemString[3].Replace("Quantity=", "");


                if (producerId.Equals(producerid1) && productId.Equals(productid1) && unitPrice.Equals(unitPrice1) && quantity.Equals(quantityKg))
                {
                    removeFromCartList(c,cartItemsAsString[i]);
                    continue;
                }

            }
            countCartItems();
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult EditItem(string producerId, string productId, string oldQuantity,string newQuantity, string unitPrice)
        {
            string userID = User.Identity.GetUserId();
            Cart c = db.Carts.Where(x => x.UserID == userID).FirstOrDefault();

            string[] cartItemsAsString = c.CartItems.Split('|');

            for (int i = 0; i < cartItemsAsString.Length; i++)
            {
                if (cartItemsAsString[i].Equals(""))
                { continue; }

                CartItem item = new CartItem();
                string[] individualCartItemString = cartItemsAsString[i].Split(',');

                string producerid1 = individualCartItemString[0].Replace("ProducerId=", "");
                string productid1 = individualCartItemString[1].Replace("ProductId=", "");



                string unitPrice1 = individualCartItemString[2].Replace("UnitPrice=", "");
                string quantityKg = individualCartItemString[3].Replace("Quantity=", "");


                if (producerId.Equals(producerid1) && productId.Equals(productid1) && unitPrice.Equals(unitPrice1) && oldQuantity.Equals(quantityKg))
                {
                    modifyItem(c, cartItemsAsString[i],oldQuantity,newQuantity);
                    continue;
                }

            }
            countCartItems();
            return RedirectToAction("Index");
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
        private void removeFromCartList(Cart c, string v)
        {
            
           Debug.WriteLine("Item's current text: " + c.CartItems);
            

        
            string stringToDelete = "|" + v + "|";

            Debug.WriteLine("Item to delete: " +stringToDelete);

          
          string newCartItems =c.CartItems.Replace(stringToDelete,"");
            c.CartItems = newCartItems;
            Debug.WriteLine("Item's updated text: " + c.CartItems);

            db.SaveChanges();
        }

        private void modifyItem(Cart c, string v, string oldQuantity,string newQuantity)
        {

            Debug.WriteLine("Item's current text: " + c.CartItems);



            string oldString = "|" + v + "|";
            string newString = oldString.Replace("Quantity=" + oldQuantity, "Quantity=" + newQuantity);
          


            string newCartItem = c.CartItems.Replace(oldString,newString);
            c.CartItems = newCartItem;
           
            countCartItems();
            db.SaveChanges();
        }
    }
}