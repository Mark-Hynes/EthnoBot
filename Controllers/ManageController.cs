﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using EthnoBot.Models;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNet.Identity.EntityFramework;

namespace EthnoBot.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private EthnoBotEntities db = new EthnoBotEntities();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;


        public ActionResult Profilepic()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase postedFile)
        {
            byte[] bytes;
            using (BinaryReader br = new BinaryReader(postedFile.InputStream))
            {
                bytes = br.ReadBytes(postedFile.ContentLength);
            }
          
            db.Images.Add(new ImageModel
            {
                Name = Path.GetFileName(postedFile.FileName),
                ContentType = postedFile.ContentType,
                Data = bytes
            });
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<ActionResult> Profilepic(string base64image)
        { //  string base64image = image.


            string userID = User.Identity.GetUserId();
            ApplicationUser user = UserManager.FindById(User.Identity.GetUserId());
            user.ImagePath = base64image;
            IdentityResult result = await UserManager.UpdateAsync(user);
            UserManager.Update(user);

            TempData["Success"] = "Image uploaded successfully";
            return RedirectToAction("Index");
                
        }

    


      /*  public ActionResult FileUpload(HttpPostedFileBase file)
        {
            if (file != null)
            { string userID = User.Identity.GetUserId();
                Seller p = db.Sellers.Where(x => x.ASPUserId == userID).First();
                // string pic = System.IO.Path.GetFileName(file.FileName);
                string pic = p.Name;
                string path = System.IO.Path.Combine(
                                       Server.MapPath("~/Images/Sellers/"), pic + ".png");
                // file is uploaded
                file.SaveAs(path);

                // save the image path path to the database or you can send image 
                // directly to database
                // in-case if you want to store byte[] ie. for DB
               // using (MemoryStream ms = new MemoryStream())
               // {
               //     file.InputStream.CopyTo(ms);
               //     byte[] array = ms.GetBuffer();
               // }

            }
            // after successfully uploading redirect the user
            return RedirectToAction("SellerIndex", "Manage");
        }
        */

        [HttpPost]
        public ActionResult Index(ApplicationUser user)
        {
            string userID = User.Identity.GetUserId();
            ApplicationUser u = UserManager.Users.Where(x => x.Id == userID).First();
            u.FirstName = user.FirstName;
            u.LastName = user.LastName;
            u.Email = user.Email;
            u.AddressLine1 = user.AddressLine1;
            u.AddressLine2 = user.AddressLine2;
            u.AddressLine3 = user.AddressLine3;
            u.Country = user.Country;
           
           
            UserManager.Update(u);
            return RedirectToAction("UserIndex");
        }


        [HttpPost]
        public ActionResult SellerIndex(Seller Seller)
        {
            string userID = User.Identity.GetUserId();
            Seller p = db.Sellers.Where(x => x.ASPUserId == userID).FirstOrDefault();
             p.FirstName = Seller.FirstName;
             p.LastName = Seller.LastName;
             p.About = Seller.About;
             p.Description = Seller.Description;
             p.Email = Seller.Email;
             p.AddressLine1 = Seller.AddressLine1;
             p.AddressLine2 = Seller.AddressLine2;
             p.AddressLine3 = Seller.AddressLine3;
            p.PostCode = Seller.PostCode;
            p.City = Seller.City;
            p.Country = Seller.Country;
             p.Mobile = Seller.Mobile;
           
            p.IsVerified = Seller.IsVerified;
           
            db.SaveChanges();
            return RedirectToAction("SellerIndex");
        }

        [Authorize]
        public ActionResult SaveSellerChanges(string name)
        {
            string userID = User.Identity.GetUserId();
            Seller p = db.Sellers.Where(x => x.ASPUserId == userID).First();
            p.FirstName = name;
            
            
            db.SaveChanges();
            return RedirectToAction("SellerIndex");
        }

        [Authorize]
        public ActionResult DeleteListing(string listingId)
        {
            string userID = User.Identity.GetUserId();
            Seller seller = db.Sellers.Where(x => x.ASPUserId == userID).FirstOrDefault();
          
                Listing l = db.Listings.Where(x => x.ListingId == listingId && x.SellerId==seller.SellerId).First();
            if(l!=null)
            db.Listings.Remove(l);

                db.SaveChanges();
            return RedirectToAction("SellerIndex");
        }
        [Authorize]
        public ActionResult MyListings()
        {
            string userID = User.Identity.GetUserId();
            Seller seller = db.Sellers.Where(x => x.ASPUserId.Contains(userID)).First();

           
            List<ListingViewModel> listingViewModels = new List<ListingViewModel>();
            try
            {

                List<Listing> listings = db.Listings.Where(x => x.SellerId.Contains(seller.SellerId)).ToList();
                for (int i = 0; i < listings.Count; i++)
                {
                    ListingViewModel lo = new ListingViewModel();
                    lo.Listing = listings.ElementAt(i);

                    string productId = listings.ElementAt(i).ProductId.Trim();
                    string listingId = lo.Listing.ListingId.Trim();
                    Product product = db.Products.Where(x => x.ProductId.Contains(productId)).First();
                    lo.Offers = db.Offers.Where(x => x.ListingId.Contains(listingId)).ToList();
                    lo.Product = product;
                    lo.Seller = seller;
                    listingViewModels.Add(lo);

                }
                return View(listingViewModels);
            }
            catch (Exception e)
            {
                return View(listingViewModels);
            }

            return View(listingViewModels);
        }
        public ActionResult MyDetails()
        {
            return View();
        }
        public ActionResult MyOrders()
        { string userId = User.Identity.GetUserId();
            List<OrderViewModel> orderViewModels = new List<OrderViewModel>();
            IEnumerable<Order> orders = db.Orders.Where(x => x.SellerId == userId||x.BuyerId==userId).ToList();
            if(orders!=null)
            foreach (var order in orders)
            {
                    string productid = order.ProductId;
                    string sellerid = order.SellerId;
                    string buyerid = order.BuyerId;
                    OrderViewModel viewModel = new OrderViewModel();
                    viewModel.Order = order;
                   viewModel.Product = db.Products.Where(x => x.ProductId ==productid).First();
                    viewModel.Seller = db.Sellers.Where(x => x.SellerId == sellerid).First();
                    viewModel.Buyer = UserManager.FindById(buyerid);

                    orderViewModels.Add(viewModel);
                }
            return View(orderViewModels);
        }

        public ActionResult MyMessages()
        {
            return View();
                {

            }
        }
        [Authorize]
        public ActionResult EditListing(string ListingId, string newQuantity, string unitPrice)
        {
            string userID = User.Identity.GetUserId();


            Listing l = db.Listings.Where(x => x.ListingId == ListingId).First();
         
            db.SaveChanges();

            return RedirectToAction("MyListings");
        }




        public ActionResult LoadListingTags(string productId)
        {
            Product p = db.Products.Where(x => x.ProductId.Contains(productId)).First();
            string productTye;
            // List<ListingTag> tags = db.ListingTags.Where(x => x.ListingTagCategoryId.Contains(listingcategoryId)).OrderBy(x=>x.Name).ToList();
            return PartialView("","");
        }
            [Authorize]
        public ActionResult AddListing()
        {
            ListingViewModel vm = new ListingViewModel();
            vm.ListingTagCategories = db.ListingTagCategories.OrderBy(x => x.Name).ToList();
            vm.ListingTags = db.ListingTags.OrderBy(x => x.ListingTagType).ToList();
            vm.NewOffer = new Offer();
           
            vm.Products = db.Products.OrderBy(x=>x.ProductType).ToList();
          
                   
            return View(vm);
                               
        }
        [Authorize]
        public ActionResult CompleteListing(string selectedTags, string productId, string description, string currency, string currencyAmount, string unitMeasurement, string unitAmount )
        {
          //  string regex = "^ (\d{ 1,5}|\d{ 0,5}\.\d{ 1,2})$";
            
            string userID = User.Identity.GetUserId();
            Seller pr = db.Sellers.Where(x => x.ASPUserId == userID).First();
            Guid guid = Guid.NewGuid();

            Listing l = new Listing
            { ListingId = guid.ToString(),
                SellerId = pr.SellerId,
                ProductId =productId,
                Description = description

            };
            db.Listings.Add(l);
            Offer offer = new Offer();
            offer.OfferId = Guid.NewGuid().ToString();
            offer.ListingId = l.ListingId;
            offer.Currency = currency;
            offer.Price = Decimal.Parse(currencyAmount);
            offer.Units = Decimal.Parse(unitAmount);
            offer.Measurement = unitMeasurement;
            db.Offers.Add(offer);

            string[] listingTagIds = selectedTags.Split(',');
            foreach (var item in listingTagIds)
            {
                ListingTagAssociation association = new ListingTagAssociation();
                association.ListingTagAssociationId = Guid.NewGuid().ToString();
                association.ListingId = l.ListingId;
                association.ListingTagId = item;
                db.ListingTagAssociations.Add(association);
            }
            db.SaveChanges();
            return RedirectToAction("MyListings");
        }






       

        public ActionResult EditSellerProfile(Seller p)
        {
            string userId = User.Identity.GetUserId();
            if (userId== p.ASPUserId && UserManager.IsInRole(userId, "Seller"))
            { 
                return View(p);
            }
            return RedirectToAction("Index","Home",null);
        }
        public ManageController()
        {

        }

        public async Task<ActionResult> IncompleteSellerIndex()
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            var model = new UserIndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(user.Id),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(user.Id),
                Logins = await UserManager.GetLoginsAsync(user.Id),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(user.Id),
                Mobile = await UserManager.GetPhoneNumberAsync(user.Id),
                AddressLine1 = user.AddressLine1,
                AddressLine2 = user.AddressLine2,
                AddressLine3 = user.AddressLine3,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Country = user.Country,
                Email = user.Email,
                ImagePath = user.ImagePath

            };
            return View(model);
        }
        public ActionResult SellerIndex()
        {

          
            string userId = User.Identity.GetUserId();
            var user = UserManager.FindById(userId);
            try { Seller seller = db.Sellers.Where(x => x.ASPUserId == userId).First();
                if (seller == null)
                {
                    return RedirectToAction("IncompleteSellerIndex", "Manage");
                }

                ManageSellerViewModel sellerViewModel = new ManageSellerViewModel();
                sellerViewModel.ImagePath = user.ImagePath;
                sellerViewModel.Seller = seller;

                return View(sellerViewModel);
            } catch
            {
                return RedirectToAction("IncompleteSellerIndex", "Manage");
            }
         
        }

    public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            
            if (await UserManager.IsInRoleAsync(User.Identity.GetUserId(), "User"))
            {
                return RedirectToAction("UserIndex", "Manage");
            }
            else if (await UserManager.IsInRoleAsync(User.Identity.GetUserId(), "Seller"))
            {
                return RedirectToAction("SellerIndex", "Manage");
            }
            else
            {
                ViewBag.StatusMessage =
              message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
              : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
              : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
              : message == ManageMessageId.Error ? "An error has occurred."
              : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
              : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
              : "";

                var userId = User.Identity.GetUserId();
                var model = new IndexViewModel
                {
                    HasPassword = HasPassword(),
                    PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                    TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                    Logins = await UserManager.GetLoginsAsync(userId),
                    BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId)
                };
                return View(model);
            }
          
        }
        public async Task<ActionResult> UserIndex(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
              message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
              : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
              : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
              : message == ManageMessageId.Error ? "An error has occurred."
              : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
              : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
              : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            
            var model = new UserIndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(user.Id),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(user.Id),
                Logins = await UserManager.GetLoginsAsync(user.Id),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(user.Id),
                Mobile = await UserManager.GetPhoneNumberAsync(user.Id),
                AddressLine1 = user.AddressLine1, AddressLine2 = user.AddressLine2, AddressLine3 = user.AddressLine3,
                FirstName = user.FirstName, LastName = user.LastName, Country = user.Country , Email=user.Email , ImagePath=user.ImagePath
          
            };
            return View(model);
        }
        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Your security code is: " + code
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
            // Send an SMS through the SMS provider to verify the phone number
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Failed to verify phone");
            return View(model);
        }

        //
        // POST: /Manage/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

#region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

#endregion
    }
}