using EthnoBot.Models;
using Microsoft.AspNet.Identity;
using PayPal.Api;
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
            try
            {
                List<CartItem> items = new List<CartItem>();
                string userID = User.Identity.GetUserId();
                Cart c = db.Carts.Where(x => x.UserId == userID).FirstOrDefault();
                if (c == null)
                {
                    AccountController.createCart(userID);
                    c = db.Carts.Where(x => x.UserId == userID).FirstOrDefault();
                }
                items = db.CartItems.Where(x => x.CartId == c.CartId).ToList();
                    //find cart items where it matches current user's cart
                for (int i = 0; i < items.Count; i++)
                {
                  

                    CartItem item = items.ElementAt(i);


                    string SellerId = item.SellerId;
                    string ProductId = item.ProductId;

                    Seller s = db.Sellers.Where(x => x.SellerId == SellerId).First();
                    Product p = db.Products.Where(x => x.ProductId == ProductId).First();
                    Listing l = db.Listings.Where(x => x.ProductId == p.ProductId && x.SellerId == s.SellerId).First();
                    item.UnitPriceKG = l.UnitPriceKG;        
                }
                countCartItems();
                return View(items);
            }
            catch (Exception) { return View("Error"); }
        }

        public ActionResult Checkout()
        {

            return View();
        }

        private Payment payment;

        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            var listItems = new ItemList() { items=new List<Item>()};
            string userId = User.Identity.GetUserId();
            Cart cart = db.Carts.Where(x => x.UserId == userId).First();
           List< CartItem> items = db.CartItems.Where(x => x.CartId == cart.CartId).ToList();
            var transactionList = new List<Transaction>();
            Payer payer = null;
            RedirectUrls redirUrls = null;
            Details details = new Details();
            Amount amount = new Amount()
            {
                currency = "EUR",
                total = "0"
                 }; 
            for (int i = 0; i < items.Count; i++)
            {


                CartItem item = items.ElementAt(i);
                  

                   Seller seller= db.Sellers.Where(x => x.SellerId == item.SellerId).First();
                   Product product = db.Products.Where(x => x.ProductId == item.ProductId).FirstOrDefault();

                   Listing l = db.Listings.Where(x => x.ProductId == product.ProductId && x.SellerId == seller.SellerId).First();


                    item.UnitPriceKG = l.UnitPriceKG;
                   
                    float total = item.UnitPriceKG * item.UnitsKG;
                    listItems.items.Add(new Item()
                    {
                        name = product.Title + " x " + item.UnitsKG,
                        currency = "EUR",
                        price = l.UnitPriceKG.ToString(),
                        quantity = item.UnitsKG.ToString(),
                        sku = "sku"
                        
                        

                    });

                     payer = new Payer() { payment_method = "paypal" };
                     redirUrls = new RedirectUrls()
                    {
                        cancel_url = redirectUrl,
                        return_url = redirectUrl
                    };

                    details.tax = (Convert.ToDouble(details.tax)+Convert.ToDouble("1")).ToString();
                    details.shipping =  (Convert.ToDouble(details.shipping)+Convert.ToDouble("10")).ToString();
                    details.subtotal = (Convert.ToDouble(details.subtotal)+ Convert.ToDouble(l.UnitPriceKG) * Convert.ToDouble(item.UnitsKG)).ToString();
                    amount.total = (Convert.ToDouble(details.tax)+ Convert.ToDouble(details.shipping)+Convert.ToDouble(details.subtotal)).ToString();
                    
                    amount.details = details;


                    

                
                
            }
            Debug.Print(listItems.items.ToList().ToString());
            transactionList.Add(new Transaction()
            {
                description = "Purchase from EthnobotanicalsIreland",
                invoice_number = Convert.ToString(new Random().Next(100000)),
                amount = amount,
                item_list = listItems

            });

            payment = new Payment();
            payment.intent = "sale";
            payment.payer = payer;
            payment.transactions = transactionList;
            payment.redirect_urls = redirUrls;
            return payment.Create(apiContext);


          
        }

        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id=payerId
            };
            payment = new Payment() { id = paymentId };
            return payment.Execute(apiContext, paymentExecution);
        }

        public ActionResult PaymentWithPaypal()
        {
            APIContext apiContext = PayPalConfiguration.GetAPIContext();
            
            try
            {
                string payerId = Request.Params["PayerID"]; //Maybe here ?!
                if (string.IsNullOrEmpty(payerId))
                {
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Cart/PaymentWithPaypal?";
                    var guid = Convert.ToString((new Random()).Next(100000));
                    var createdPayment = CreatePayment(apiContext, baseURI + "guid=" + guid);

                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = string.Empty;
                    while (links.MoveNext())
                    {
                        Links link = links.Current;
                        if (link.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = link.href;
                        }
                    }
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    var guid = Request.Params["guid"];
                    string sessionGuid = (string)Session[guid];
                    var executePayment = ExecutePayment(apiContext, payerId,sessionGuid);
                    if (executePayment.state.ToLower() != "approved")
                    {
                        return View("Failure");
                    }
                }
            }
            catch (Exception ex)
            {
                PaypalLogger.Log("Error: " + ex.Message);
                return View("Failure");
            }
            createOrder();
            return View("Success");
        }
        protected void createOrder()
        {

        }
        [Authorize]
        public ActionResult RemoveFromBasket(string cartItemId)
        {
            string userID = User.Identity.GetUserId();
            Cart c = db.Carts.Where(x => x.UserId == userID).FirstOrDefault();

            List<CartItem> items = db.CartItems.Where(x => x.CartId == c.CartId && x.CartItemId==cartItemId).ToList();

            for (int i = 0; i < items.Count; i++)
            {
                CartItem item = items.ElementAt(i);

                items.Remove(item); 
         }
            countCartItems();
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult EditItem(string cartItemId, string newQuantity)
        {
            string userID = User.Identity.GetUserId();
            Cart c = db.Carts.Where(x => x.UserId == userID).FirstOrDefault();

            CartItem item = db.CartItems.Where(x => x.CartItemId == cartItemId).First();

            if (item != null)
            {
                item.UnitsKG = float.Parse(newQuantity);
                db.SaveChanges();
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
                    Cart c = db.Carts.Where(x => x.UserId == userID).First();
                    if (c == null)
                    {
                        AccountController.createCart(userID);
                        c = db.Carts.Where(x => x.UserId == userID).FirstOrDefault();
                    }
                    int count = db.CartItems.Where(x=>x.CartId==c.CartId).ToList().Count; 
                    
                   
                    Session["CartItemCount"] = count;
                }
            }
            catch (Exception e)
            {
                Exception ex = e;

            }
        }
        private void removeFromCartList(string cartItemId)
        {
                   CartItem item = db.CartItems.Where(x => x.CartItemId == cartItemId).First();
                   db.CartItems.Remove(item);
                   db.SaveChanges();
        }

        private void modifyItem(string cartItemId,string newQuantity)
        {

            CartItem item = db.CartItems.Where(x => x.CartItemId == cartItemId).First();
            if (item != null)
            {
                item.UnitsKG = float.Parse(newQuantity);
                db.SaveChanges();
            }

            countCartItems();
            
        }
    }
}