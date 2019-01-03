using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using EthnoBot.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.Validation;
using System.Net;
using System.Net.Mail;
using EthnoBot.Utils;
using System.Web.Security;

namespace EthnoBot.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private static EthnoBotEntities db = new EthnoBotEntities();

        public AccountController()
        {
        }
        public void sendVerifyCompanyEmail(Seller p)
        {
            GMailer.GmailUsername = "mrkhynes1@gmail.com";
            GMailer.GmailPassword = "bkrytjdxkhxzpueh";

            GMailer mailer = new GMailer();
            mailer.ToEmail = "martinoneill360@gmail.com";
            mailer.Subject = "ADMINISTRATOR INPUT REQUIRED";
           
            mailer.Body = "Hello Admin"+ Environment.NewLine + Environment.NewLine + "A new user has registered to be a seller on the EthnoBotanicalsIreland website" + Environment.NewLine+ Environment.NewLine+
                "Please navigate to 'Manage Sellers tab to verify their details." + Environment.NewLine+
                "Once you change the sellers's value 'Verified' to 'True', the seller will be listed on the website as a Seller." + Environment.NewLine+
                Environment.NewLine+"The seller has entered the following details:" + Environment.NewLine+

                Environment.NewLine+ Environment.NewLine+"Seller Name: " + p.FirstName+" " +p.LastName+ Environment.NewLine +
                "Company About: "+ p.About+ Environment.NewLine +
                "Company Description: "+ p.Description+ Environment.NewLine +
                "Address Line 1: " + p.AddressLine1+ Environment.NewLine +
                "Address Line 2: " + p.AddressLine2+ Environment.NewLine +
                "Address Line 3: " + p.AddressLine3+ Environment.NewLine +
                "PostCode: " + p.PostCode + Environment.NewLine +
                "City: " + p.City + Environment.NewLine +
                "Country: " + p.Country + Environment.NewLine +
                "Email: "  +p.Email+ Environment.NewLine +
                "Company Mobile: "+ p.Mobile+ Environment.NewLine;
            mailer.IsHtml =false;
            mailer.Send();

        
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
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
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult RegisterSeller()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            using (var context = new ApplicationDbContext())
            {
                if (ModelState.IsValid)
                { 
                    bool p = checkIfEmailExists(model.Email);
                    if (p == false)
                    {

                        var roleStore = new RoleStore<IdentityRole>(context);
                        var roleManager = new RoleManager<IdentityRole>(roleStore);

                        var userStore = new UserStore<ApplicationUser>(context);
                        var userManager = new UserManager<ApplicationUser>(userStore);

                        ApplicationUser user ;

                        string imagepath = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAALQAAAC0CAMAAAAKE/YAAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAMAUExURYOCbBNlviCU8f/BHRRkvxp82E2WwdyYJSCV8hRlvyCV8iCV8hZtyHaWlf6YADaV2xVlwP/jNR+Q7SCV8hVkv/+rDRVlvyCV8yGU8h+O66uXWyCV8xVkvxl20hRkwESWzJyXbB2I5P/QKBVkwCCV8hyE4ISXhRRkvx+V9BRkv5OXdvSXCv+9GiCV8h+V8iCV8h6L5xdvyxVlvj2W1Bp41CCV8v/qOtWXLhl51BVkvxVkvxp51BZowyaV7DKW4BVkvx+T7x+V8iCS7xuA3ImXgSCV8hRkv1SWuriXThVkv/+yE//LJKOXZB6M6X2XjSCV8iCV8hhyziCW8tmXKRVlviCW8hhuyRRkwI2FZR+O6yCW8xVkvxVkvxRkvxdtyCCV8v/YLbOXUo6XfBVkvx+Y8RRmwTmW2P/EHxRkwIGXiUeWyOCYISGW8iGW8hRkvxhvyxRkv0GWzxRkvxVkvyCW9BRlwKCXaCCV8hRkvxRkwLCXVtKYMSGV8viXBxRkv3qWkRRkvyGV8yCW8hRkwBNkwBRkwBRjwBRkv5eXcRRkwCKY9fWYChZkwJGXeHSXmIKFcRNlwNqYKB6N6qWYYiCV8yCS79aYLRRkwBVkwLiYTQAAAJkzM5kzZpkzmZkzzJkz/5lmAJlmM5lmZplmmZlmzJlm/5mZAJmZM5mZZpmZmZmZzJmZ/5nMAJnMM5nMZpnMmZnMzJnM/5n/AJn/M5n/Zpn/mZn/zJn//8wAAMwAM8wAZswAmcwAzMwA/8wzAMwzM8wzZswzmcwzzMwz/8xmAMxmM8xmZsxmmcxmzMxm/8yZAMyZM8yZZsyZmcyZzMyZ/8zMAMzMM8zMZszMmczMzMzM/8z/AMz/M8z/Zsz/mcz/zMz///8AAP8AM/8AZv8Amf8AzP8A//8zAP8zM/8zZv8zmf8zzP8z//9mAP9mM/9mZv9mmf9mzP9m//+ZAP+ZM/+ZZv+Zmf+ZzP+Z///MAP/MM//MZv/Mmf/MzP/M////AP//M///Zv//mf//zP///xWQUB4AAACbdFJOU/81N//F////wXt88v/////+///+m/9Vm1vU/9uz/9X/////bKz//6BIiv///4ls4vf/Sf+vyv//tOVh/////6nuQv///6SF//92//////+zgv/T/z1Un5b//3DLXLigkf///9o7QP//Ov///7pMvf+Y/8DxYHv/6WWB//89/9//rUZpjlpoTqT/4jb/Uv///3P/2f907P/otP8A+/4U0QAAAAlwSFlzAAAOwwAADsMBx2+oZAAACxhJREFUeF7tnf1/FMUdx6V4IZKlXC6gVHrCqSdSwBhTbCEXFMJDBU5zosWgFHxoIKEmqEUUOMVHtNG29oG2+9d2ZvezdzO735n5zmbZF3m97v0T3O3MvDP57uw87eSBcA0ykC6LgXRZDKTLYiBdFgPpshhIl8VAuiwG0mUxkC6LeyF9eHb4+ljE9eHZw/iwSIqXvgDhhOv78EVxFC19DKoaj+DLoihW+twINFOMfIkLiqFQ6VfgSPAkLimEIqW3Q5BkOy4qggKlUzdgmnFcVgCrlD741Z3zS+d3HX/YUc8SUdcPH991fmnpzlcHkTwnq5C+vOU/1T7PQs3Cs7hUcuDqZWSTg9zSz72J4mMqELNSwcUxbz6HrLzJK/0hSgYBtBwEuBx8iMx8ySf9CUrt0YCVgwYu7/ENMvQjl/QWFNmjAyknHSTocRVZepFH+gUU2IdZ0URVV19Apj7kkM46t6DEoIUkfXYhWw/8pa+hMAVW0xEzhSQKLyFjPt7Sl1GUCjs6qPioVtchazbe0u+iJBUIsUASlceQNRtf6SdQkAqzkY5JNdURTyBzLr7SB1COSh0+LDKNnuAjZM7FU/ogitGYgg8L4k6sVr9G9kw8pR9EKRoejUe6/wE8n+ee0ihEx0uarOkqsmfiJ/0WytBZfXhU30IBPPykf1ufqncy9z+75yHJ3oitYDn4PQrgwZa+deFVFDs21p7RivZ4iqee4616cw6fbz/GH7EzpTemB4C1ilJ6DR8ymEMSSSf9JB1+HcU5YElvJKcz+u1AE58waCKJUG7jI5XxbSjSCkP6PdOQtZ1Et8fTpY4krRl8kGYYpdpwS29EbhQIbf+uaZCEcpaRP6NgM07pR5AXDSrOdxAQWO+CWRRtxCVNTigqxNbs+Igvd/WwLqBwEw7pC8jGTBQh7PiInY/gf0Y2ongDdultyMRGZM1sP6K2o8VoId+HAI1d2jBzqyPbkAn828GEdKaaujQjEKCxSlumbhXmZIvAMRlry4rm3bSvQIHEJv0FMnAhmwTWrShvQ26X8GlIUNik9yO9E/lsZARqjfvDSfZDgsIifQvJGYibkdE/FT+bx2PIsixmkbY/VjRqLY6OiH1W6MccgwaBRZrVdAAR1qbORI8ZxjUKlgbELO0RHYKKeyIh8BstWNpqs7Sto0Qw4XrANKst55NQw7xoapZ29TpS1FrL+JeBZZ+AlpibarM0u8EDTfWxMT05dOLE0OQ0/ito+A3aBeblMLO0Y4UtSwdRvbjwvys/B1d+t7Aj/nTCa/JMYr4TzdI+jUfEkZaI6vmjG+DbY8PRefl78AwOAUSyFCg91gwm/9mFqUb39GTgGxwCiGQpMDzGpt8glSXdN5Tw5gKRLGbp/jQHk6FeJFNcGcJlbPLENK9f2mfBWM0x3ZdxIRdzl8ksvQ9pmeyEm4WduJSJufNhln4faXk8CjErj+JiHuZpMrN0iLQsGPUs+Q6Xs4AGgUX6H0jMYAFSTjzi+t/QIDBLn+VPdg057sE+XX4bUjGvQBulz/InQ6czbd3WUxdHn39+9OKlrfigxwZ2e90yr5ubpF8S4znuDP+PEEpYL4QTRtfjw4R3kMiFGDJU/w6ZNAbp10SSatU8S6gyqQfHSUVZMnoSX8R0RUeEQzRTadh9Y5D+LJLmjTROQyfmElwVLuGrmNNIZidemnkXOilo6U+jJLwx3Watoh+HqMbj+DKiuxkJbSRL6A9BSIeWRhLWdNBRyEQQ9SzRAvspJLQgRvcx9FouKZ1UNGvi7QxcJCchmUGN67tIaKG/gEZWNSkdR3SE03oHVCJS92CfUVwQ8QGSmjiiLPp9DyUNSvqPSBAhhyM2XoaJZC8UCfbiEsnnSGqgpi1UvgYpFUp6N64H9ub6bZhIjBWtV/XbSErTTOI55jikVCjp9Kp9YLsdlSHhVgiSKM/Gn5CUopYsfyU8CCkVSvojXN8nMMeI0uAZmo4YpbHuImmWdnblnGo/KGlcrtOZIW/JSYhILsKP5CIuktAPxcaUFswJkFJhS0uCqZlmSn0RHhJLSOtBvYjECXONmTopLIGUCiEd9zvMtIJOfaoy02i3a7WxE/CQQM8ALpKckKa1dqM5U5nqTBh1Y4j+ByFNbWJTCCakcrPRjqYTc0rX5trCuSI3YuiNRRaetDE8RHQ00n3sIsJDRAcdzhGQUuFKBxW63ZuHh2R1N2JqE0kPSKlQ0tkmb8r8MC+0yWtmK5zb5KUfLnXbuOsnmAi4D5czSErRSGtTG8ko6X4nT+LofOR4jN9AUprU8+VTSKlQ0lrzYd/lMDb2OUwkxXSY9M4HNeKipNWu6YRrneQDmETwuqaTSGqirVh/BiUNUvohpBD1jHws/AYqEtYg4CYSmlGsr0FJg5TutR+cXQ5PwSViPSRTaMOtBSS00Ntl7THc6lU1Z8fAoRXIRJyCpsYpfBmx5xAS2pCzHhJ6FzstHT4WJeEtOeiTj0Rd6/M1vAlfuTPEcwohXCeTMFff57Wqdk3WrHBmEJJNh7BJY5AOvxFJHKPDHtokgmCvoj2qtnWSo0jkQjbXpld3TNLhcf42h2nlqRiz9ZJpAvIMdwKyRg8PI4zS4W7+ItqiHiAWVrAUyqC+GyJZzNLhi0jNYBOcnPwVCRj8AhoEFmkkZvEdpBz8iMtZQIPALH0OaXl8DC0rH+NiHnn2ezyJtEx+BTELXvWcb7+H74rtJsfduOIRzxF59nt4L+gvqvOnGc6k5w2cvAqRLGZpJPVg+oE9MMyw52/+C/p5NqkgqQeVYPNOUntl52bLxJqJPAv6SMqnLTuFhxZuwrTHzQXRr2t47roSlCIdYMQwuenGXYzRu3dvbMI4JfDb3ybIEx7jSMulovW+53cMDe1Q5zcaPu9oROTZOvEnpGXScO2hXuYMg1TMB1WYpf32exxpuYY5Te7O8ATzHnuz9NNIy6PD2bbpt/nqZxDJYpb2erpwNr+KazhjzoRcG2R99m22WWOzFmt0n2A56MYifRip3YiA5vzqK15vAZqjwybN320q52gZdSi317PD2vYOl02aeyvKVwLYLzJww/oLSFDYpJld6uiNG/caukC+MtLibSLJ/coIrwGJXl5hxqqcgmHt7rU0HQK79OvIw0K8y4H5iI5+KZwf0P4upV3avcc+Xn1nT5FEP6Hb2nEcmUPa9ZocdjmwX8LmvQnoOovMJR3OIiOSZJcD+0mHOVy7tfP8NKd0uM18NyaT3+zoQHzYI+QcCjbjlg7DYeSWJplE5r+QlcSHaENMbeT2v6BUCxzpcBs1IGj31yo9uvf9V7DJZ+PIv1CkFZa0aPvStd1Ql1c9ukHyUQ5alXS6cZYyW1rwzLHk/fF2s47QjPEI6V5Qx6ibSPbvew8lOeFLS/4QLBO7Bjy6bnglVyXo1Ov1X6MAHn7Sa/KoDHpXhdcoipZG9kw8pcnjX7xqun/sg8K9Pf5lTR60Qx5ptLobUXAAmXPxlV6Th0etyWO61uaBaGvy6LnCD/m7g2w9yCEd3kZxfXwHAQq3kakPeaSzB1ey50PjHREKW5ClF7mkoz0KGsyqzlT0J8jQj3zSBR3GuoTMfMkrHX67+mNvv0VW3uSWDsN1V9Vnuu8Bw2e9W+c+q5CWREc5/3LXtR94Rzn/cG3L7aX/3tnt2UFKs0ppFcdy2P1zaLbGWjye3Do1bJ269aVQ6fBLw2zU/XzkvoA86Ob+/uMGkn2pQxTG7/8/IyG51fuDHePDs7fwYZHcC+l7zkC6LAbSZTGQLouBdFkMpMtiIF0WA+myGEiXxUC6HMLw/6vA7wb/TZsqAAAAAElFTkSuQmCC"; 
                            user = new ApplicationUser() { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, IsSeller = model.isSeller, Country=model.Country, ImagePath=imagepath };
                            //add details to seller table + Set up shop!
                         
                        
                      
                        var result = await UserManager.CreateAsync(user, model.Password);

                      
                        try
                        {
                           
                            createCart(user.Id);
                            if (result.Succeeded)
                            {
                                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                                ViewData["LoginErrorMessage"] = null;
                                if (!user.IsSeller)
                                {
                                    userManager.AddToRole(user.Id, "User");
                                    return RedirectToAction("Index", "Home");
                                }
                                else {
                                    userManager.AddToRole(user.Id, "Seller");
                                    return RedirectToAction("RegisterSeller", "Account"); }
                            }
                            else
                            {
                                ViewData["LoginErrorMessage"] = result.Errors.ToString();
                                return RedirectToAction("Register", "Account");
                            }
                            //AddErrors(result);

                        }
                        catch (Exception e)
                        {
                            ViewData["LoginErrorMessage"] = e.ToString();
                            return RedirectToAction("Register", "Account");
                        }
                    }
                    else {
                        ViewData["LoginErrorMessage"] = "This email is already registered with an existing account.";
                        return View(model);
                    }
                }
                // If we got this far, something failed, redisplay form
                return View(model);
            }
        }

        private bool checkIfEmailExists(string email)
        {
           if (UserManager.Users.Any(u => u.Email == email))
                return true;
            else
                return false;
        }

        public  static void createCart(string id)
        {
            Cart userCart = new Cart(); 
            Guid cartID = Guid.NewGuid();
            userCart.CartId = cartID.ToString();
            userCart.UserId = id;
            
        
                
              
       
            db.Carts.Add(userCart);
            db.SaveChanges();

        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterSeller(RegisterSellerViewModel model)
        {
            using (var context = new ApplicationDbContext())
            {
                if (ModelState.IsValid)
                {
                    string userId = User.Identity.GetUserId();

                    ApplicationUser user = UserManager.Users.Where(u => u.Id == userId).First();
                    Guid guid = Guid.NewGuid();
                    string idstring = guid.ToString();
                    Seller seller = new Seller();
                    seller.IsVerified = false;
                    seller.SellerId = idstring;
                    seller.FirstName = user.FirstName;
                    seller.LastName = user.LastName;
                    seller.Email = user.Email;
                    seller.About = model.About;
                    seller.Description = model.Description;
                    seller.AddressLine1 = model.AddressLine1;
                    seller.AddressLine2 = model.AddressLine2;
                    seller.AddressLine3 = model.AddressLine3;
                    seller.ASPUserId = userId;
                    seller.Mobile = model.Mobile;
                    seller.City = model.City;
                    seller.PostCode = model.PostCode;
                    seller.Country = model.Country;
                    seller.ImagePath = "data:image/png;base64,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";
                        AddSellerToDatabase(seller);
                    user.IsSeller = true;
                    UserManager.UpdateAsync(user);
                        sendVerifyCompanyEmail(seller);
                    
                        return RedirectToAction("Index", "Home");
                 
                }
                // If we got this far, something failed, redisplay form
                return View(model);
            }
        }
        public void AddSellerToDatabase(Seller p)
        {
            try {
                if (ModelState.IsValid)
                {
                    db.Sellers.Add(p);
                    db.SaveChanges();
                }
            }
            catch (DbEntityValidationException e)
            {
                Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!HEY FUCK YOU OVER HERE!!!!!!!!!!!!!!!!!!");
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                
            }
        }
        

       
        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
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

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                Id = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string Id { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (Id != null)
                {
                    properties.Dictionary[XsrfKey] = Id;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}