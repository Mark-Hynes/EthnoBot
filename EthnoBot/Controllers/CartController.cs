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
                Cart c = db.Carts.Where(x => x.UserID == userID).FirstOrDefault();
                if (c == null)
                {
                    AccountController.createCart(userID);
                    c = db.Carts.Where(x => x.UserID == userID).FirstOrDefault();
                }
                string[] cartItemsAsString = c.CartItems.Split('|');

                for (int i = 0; i < cartItemsAsString.Length; i++)
                {
                    if (cartItemsAsString[i].Equals(""))
                    { continue; }

                    CartItem item = new CartItem();
                    string[] individualCartItemString = cartItemsAsString[i].Split(',');

                    int producerid = Int32.Parse(individualCartItemString[0].Replace("ProducerId=", ""));
                    int productid = Int32.Parse(individualCartItemString[1].Replace("ProductId=", ""));

                    item.producer = db.Producers.Where(x => x.ProducerId == producerid).FirstOrDefault();
                    item.product = db.Products.Where(x => x.ProductId == productid).FirstOrDefault();
                    ProducerProduct pp = db.ProducerProducts.Where(x => x.ProductId == item.product.ProductId && x.ProducerId == item.producer.ProducerId).First();


                    item.unitPrice = pp.UnitPrice;
                    item.quantityKg = Int32.Parse(individualCartItemString[3].Replace("Quantity=", ""));
                    item.total = item.unitPrice * item.quantityKg;
                    items.Add(item);
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
            Cart cart = db.Carts.Where(x => x.UserID == userId).First();
            string[] cartItemsAsString = cart.CartItems.Split('|');
            var transactionList = new List<Transaction>();
            Payer payer = null;
            RedirectUrls redirUrls = null;
            Details details = new Details();
            Amount amount = new Amount()
            {
                currency = "EUR",
                total = "0"
                 }; 
            for (int i = 0; i < cartItemsAsString.Length; i++)
            {
                if (!string.IsNullOrEmpty(cartItemsAsString[i]))
                {


                    CartItem item = new CartItem();
                    string[] individualCartItemString = cartItemsAsString[i].Split(',');

                    int producerid = Int32.Parse(individualCartItemString[0].Replace("ProducerId=", ""));
                    int productid = Int32.Parse(individualCartItemString[1].Replace("ProductId=", ""));

                    item.producer = db.Producers.Where(x => x.ProducerId == producerid).FirstOrDefault();
                    item.product = db.Products.Where(x => x.ProductId == productid).FirstOrDefault();

                    ProducerProduct pp = db.ProducerProducts.Where(x => x.ProductId == item.product.ProductId && x.ProducerId == item.producer.ProducerId).First();


                    item.unitPrice = pp.UnitPrice;
                   item.quantityKg = Int32.Parse(individualCartItemString[3].Replace("Quantity=", ""));
                    item.total = item.unitPrice * item.quantityKg;
                    listItems.items.Add(new Item()
                    {
                        name = item.product.Title + " x" + item.quantityKg,
                        currency = "EUR",
                        price = item.unitPrice.ToString(),
                        quantity = item.quantityKg.ToString(),
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
                    details.subtotal = (Convert.ToDouble(details.subtotal)+ Convert.ToDouble(item.unitPrice) * Convert.ToDouble(item.quantityKg)).ToString();
                    amount.total = (Convert.ToDouble(details.tax)+ Convert.ToDouble(details.shipping)+Convert.ToDouble(details.subtotal)).ToString();
                    
                    amount.details = details;


                    

                }
                
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
                int producerID = Convert.ToInt32(producerid1);
                int productID = Convert.ToInt32(productid1);

                ProducerProduct pp = db.ProducerProducts.Where(x => x.ProductId == productID && x.ProducerId == producerID).First();


                item.unitPrice = pp.UnitPrice;
                string unitPrice1 = individualCartItemString[2].Replace("UnitPrice=", "");
                string quantityKg = individualCartItemString[3].Replace("Quantity=", "");


                if (producerId.Equals(producerid1) && productId.Equals(productid1)&& quantity.Equals(quantityKg))
                {
                    removeFromCartList(c,cartItemsAsString[i]);
                    continue;
                }

            }
            countCartItems();
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult EditItem(string producerId, string productId, string oldQuantity,string newQuantity)
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

                int productID = Convert.ToInt32(productid1);
                int producerID = Convert.ToInt32(producerid1);
                ProducerProduct pp = db.ProducerProducts.Where(x => x.ProductId == productID && x.ProducerId == producerID).First();


           
                string unitPrice1 = pp.UnitPrice.ToString();
                string quantityKg = individualCartItemString[3].Replace("Quantity=", "");


                if (producerId.Equals(producerid1) && productId.Equals(productid1) && oldQuantity.Equals(quantityKg))
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