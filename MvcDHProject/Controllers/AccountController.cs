using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using MimeKit;
using MvcDHProject.Models;
using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MvcDHProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly  UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly MVCCoreDbContext context;
        public  AccountController(UserManager<IdentityUser>userManager,SignInManager<IdentityUser>signInManager,MVCCoreDbContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.context = context;
        }
       public ViewResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserViewModel userModel)
        {
            if (ModelState.IsValid)
            {
                IdentityUser identityUser = new IdentityUser
                {
                    Email = userModel.Email,
                    UserName = userModel.Name,
                    PhoneNumber = userModel.Mobile
                };
                var result = await userManager.CreateAsync(identityUser,userModel.Password);
                if (result.Succeeded)
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(identityUser);
                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { UserId = identityUser.Id, Token = token }, Request.Scheme);
                    SendMail(identityUser,confirmationLink,"Email Confirmation Link");

                    TempData["title"] = "Email Comfirmation";
                    TempData["message"] = "We have sent an email verification link go & click on the link to confirm your email.";
                    return RedirectToAction("DisplayMessage");
                }
                else
                {
                    foreach (var error  in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                        return View(userModel);
                    }
                }
            }
            return View(userModel);
        }

        public void SendMail(IdentityUser identityUser,string confirmationLink,string subject)
        {
            StringBuilder mailBody = new StringBuilder();

            mailBody.Append("Hello " + identityUser.UserName+ "<br/><br/>");
            mailBody.Append(subject);
            if(subject =="Email Confirmation Link")
            {
               mailBody.Append("<br/>" + "Click below to confirm your email.");
            }
            else if(subject =="Reset Password Link")
            {
               mailBody.Append("<br/>" + "Click below to Reset your password.");
            }
            mailBody.Append("<br/>" + confirmationLink + "<br/><br/>");
            mailBody.Append("Best Regards <br/><br/>");
            mailBody.Append("Customer Support");

            BodyBuilder bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody=mailBody.ToString();

            MailboxAddress toAddress = new MailboxAddress(identityUser.UserName,identityUser.Email);
            MailboxAddress fromAddress = new MailboxAddress("Customer Support","chakridol143@gmail.com");

            MimeMessage mimeMessage = new MimeMessage();
            mimeMessage.From.Add(fromAddress);
            mimeMessage.To.Add(toAddress);
            mimeMessage.Subject = subject;
            mimeMessage.Body = bodyBuilder.ToMessageBody();

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Connect("smtp.gmail.com", 465, true);
            smtpClient.Authenticate("chakridol143@gmail.com", "rgbc sywg rgof trgn");
            smtpClient.Send(mimeMessage);
        }

        public async Task<IActionResult> ConfirmEmail(string userId,string token)
        {
            if(userId != null && token != null)
            {
                var User = await userManager.FindByIdAsync(userId);
                if(User != null)
                {
                    var result = await userManager.ConfirmEmailAsync(User, token);
                    if (result.Succeeded)
                    {
                        TempData["title"] = "Email Confirmation Success";
                        TempData["message"] = "Now you can login to your application.";
                        return View("DisplayMessage");
                    }
                    else
                    {
                        StringBuilder errors = new StringBuilder();
                        
                        foreach (var error in result.Errors)
                        {
                            errors.Append(error.Description);
                        }
                        TempData["title"] = "Email Confirmation Failed";
                        TempData["message"] = errors.ToString();
                        return View("DisplayMessage");
                    }
                }
                else
                {
                    TempData["title"] = "Invalid User Id";
                    TempData["message"] = "The requested link has Invalid User Id go & check the link.";
                    return View("DisplayMessage");
                }
            }
            else
            {
                TempData["title"] = "Invalid Email Confirmation Link";
                TempData["message"] = "The requested link has no User Id or Token go & check the link.";
                return RedirectToAction("DisplayMessage");
            }
        }

        public ViewResult DisplayMessage()
        {
            return View();
        }
        public ViewResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginModel)
        {
            if (ModelState.IsValid)
            {
                
                var User = await userManager.FindByNameAsync(loginModel.Name);
                
                if(User != null && await userManager.CheckPasswordAsync(User,loginModel.Password) && User.EmailConfirmed == false)
                {
                    ModelState.AddModelError("", "You email is not confirmed");
                    return View(loginModel);
                }

                var result = await signInManager.PasswordSignInAsync(loginModel.Name, loginModel.Password, loginModel.RememberMe, false);
                if (result.Succeeded)
                {
                    if (string.IsNullOrEmpty(loginModel.ReturnUrl))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                      return LocalRedirect(loginModel.ReturnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Login Attempt.");
                    return View(loginModel);
                }
            }
           return RedirectToAction("Index","Home");
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        public ViewResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel forgotModel)
        {
            if (ModelState.IsValid)
            {
                if (forgotModel.Name != null)
                {
                    var user = await userManager.FindByNameAsync(forgotModel.Name);
                    if(user != null)
                    {
                        var token = await userManager.GeneratePasswordResetTokenAsync(user);
                        var confirmationLink = Url.Action("ChangePassword", "Account", new { UserName = user.UserName, Token = token }, Request.Scheme);
                        SendMail(user, confirmationLink, "Reset Password Link");

                        TempData["title"] = "Reset Password Comfirmation";
                        TempData["message"] = "We have sent an email verification link go & click on the link to confirm your email.";
                        return RedirectToAction("DisplayMessage");

                    }
                    else
                    {
                        TempData["title"] = "Invalid User Name";
                        TempData["message"] = "We cannot find your User name in our records please enter a valid user name.";
                        return RedirectToAction("DisplayMessage");
                    }
                }
            }
            return View(forgotModel);
         }
        [HttpGet]
        public ViewResult ChangePassword()
        {
            return View();
        }
        
        public async Task<IActionResult> ChangePassword(ChangePasswordModel cpModel)
        {
            if(ModelState.IsValid)
            {
                if (cpModel.UserName != null)
                {
                    var User = await userManager.FindByNameAsync(cpModel.UserName);
                    if(User != null)
                    {
                        var result = await userManager.ResetPasswordAsync(User,cpModel.Token, cpModel.Password);
                        if (result.Succeeded)
                        {
                            TempData["title"] = "Reset Password Success";
                            TempData["message"] = "Now you can login with the new password which you have created just now.";
                            return RedirectToAction("DisplayMessage");
                        }
                     else{
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }
                    else
                    {
                        TempData["title"] = "Invalid User Name";
                        TempData["message"] = "We cannot find your User name in our records please enter a valid user name.";
                        return RedirectToAction("DisplayMessage");
                    }
                }
                else
                {
                    TempData["title"] = "Invalid User Name";
                    TempData["message"] = "We cannot find your User name in our records please enter a valid user name.";
                    return RedirectToAction("DisplayMessage");
                }
            }
            
            return View(cpModel);
        }
        
        public IActionResult ExternalLogin(string provider,string returnUrl)
        {
            var url = Url.Action("CallBack", "Account", new { ReturnUrl = returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, url);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> CallBack(LoginViewModel model,string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = "~/";
            }
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError("", "Error loading external login information.");
                return View("Login", model);
            }
            var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, true);
            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (email != null)
                {
                    var user = await userManager.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
                    if (user == null)
                    {
                        user = new IdentityUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                            PhoneNumber = info.Principal.FindFirstValue(ClaimTypes.MobilePhone),
                        };
                        var identityResult = await userManager.CreateAsync(user);
                    }
                    await userManager.AddLoginAsync(user, info);
                    await signInManager.SignInAsync(user, false);
                    return LocalRedirect(returnUrl);
                }
                TempData["Title"] = "Error";
                TempData["Message"] = "Email claim not received from third party provided.";
                return RedirectToAction("DisplayMessages");
            }

        }
   

    }


}




