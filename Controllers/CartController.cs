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
        public List<CartItem> items = new List<CartItem>();
        static List<CartItemViewModel> cartItemViewModels;

        private EthnoBotEntities db = new EthnoBotEntities();
        // GET: Cart
        [Authorize]
        public ActionResult Index()
        {
            try
            {
                CartItemViewModels = new List<CartItemViewModel>();
                items = new List<CartItem>();
                
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


                    string listingId = item.ListingId;
                    string cartId = item.CartId;
                    Listing l = db.Listings.Where(x => x.ListingId == listingId).First();

                    Seller s = db.Sellers.Where(x => x.SellerId == l.SellerId).First();
                    Product p = db.Products.Where(x => x.ProductId == l.ProductId).First();
                    CartItemViewModel m = new CartItemViewModel();
                    m.CartItem = item;
                    m.Listing = l;
                    m.Product = p;
                    m.Seller = s;
                   // m.Subtotal = Convert.ToDecimal(m.Listing.UnitPriceKG * m.Listing.UnitsKG);
                    CartItemViewModels.Add(m);
                }
                countCartItems();
                return View(CartItemViewModels);
            }
            catch (Exception) { return View("Error"); }
        }

        public ActionResult Checkout()
        {

            return View();
        }

        private Payment payment;

        public static List<CartItemViewModel> CartItemViewModels { get => cartItemViewModels; set => cartItemViewModels = value; }

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

                Listing l = db.Listings.Where(x => x.ListingId == item.ListingId).First();

                Seller seller = db.Sellers.Where(x => x.SellerId == l.SellerId).First();
                Product product = db.Products.Where(x => x.ProductId == l.ProductId).FirstOrDefault();





                decimal total = 10; //Convert.ToDecimal(l.UnitPriceKG * item.UnitsKG);
                    listItems.items.Add(new Item()
                    {
                        name = product.Title + " x " + item.UnitsKG,
                        currency = "EUR",
                        price = "10",
                        quantity = item.UnitsKG.ToString(),
                        sku = "sku"
                        
                        

                    });
                
                     payer = new Payer() { payment_method = "paypal" };
                     redirUrls = new RedirectUrls()
                    {
                        cancel_url = redirectUrl,
                        return_url = redirectUrl
                    };
                List<Models.Order> orders = new List<Models.Order>();
                Guid guid =  Guid.NewGuid();
                string orderId = guid.ToString();
                double Subtotal = 0;
                double Total = 0;
                for (int j = 0; j < CartItemViewModels.Count; j++)
                 {
                  Models.Order order = new Models.Order();
                    //Find unix timestamp (seconds since 01/01/1970)
                    long ticks = DateTime.UtcNow.Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks;
                    ticks /= 10000000; //Convert windows ticks to seconds
                    order.Timestamp =  DateTime.UtcNow.ToString();
                    order.OrderItemId = Guid.NewGuid().ToString();
                    order.OrderId = orderId;
                    order.SellerId = seller.SellerId;
                    order.ProductId = CartItemViewModels.ElementAt(j).Product.ProductId;
                    order.Status = 0; 
                    order.BuyerId = User.Identity.GetUserId();
                    order.UnitsKG = CartItemViewModels.ElementAt(j).CartItem.UnitsKG;
                    order.UnitPriceKG = 10;//CartItemViewModels.ElementAt(j).Listing.UnitPriceKG;
                    order.TotalPrice = (order.UnitPriceKG * order.UnitsKG);
                    Subtotal +=Convert.ToDouble(order.TotalPrice);
                    db.Orders.Add(order);
                    db.SaveChanges();
                    RemoveFromBasket(cartItemViewModels.ElementAt(j).CartItem.CartItemId);
                    
                }

                double Shipping = 2;
                double Tax = (Subtotal * .15);

                Total = Subtotal + Tax+ Shipping;


                details.tax =  Tax.ToString();
                    details.shipping =  Shipping.ToString();
                    details.subtotal = Subtotal.ToString();
                    amount.total =Total.ToString();
                    
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
       
            return View("Success");
        }
 
     
        [Authorize]
        public ActionResult RemoveFromBasket(string CartItemId)
        {
            CartItem item = db.CartItems.Where(x=>x.CartItemId==CartItemId).First();

                        {
                 db.CartItems.Remove(item);
                db.SaveChanges();
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
                item.UnitsKG = Convert.ToDecimal(newQuantity);
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
                        c = db.Carts.Where(x => x.UserId == userID).First();

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
                item.UnitsKG = Convert.ToDecimal(newQuantity);
                db.SaveChanges();
            }

            countCartItems();
            
        }
    }
}