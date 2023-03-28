using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FullCoreProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using FullCoreProject.Models;

namespace FullCoreProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var RegisteredUser = new ApplicationUser { UserName=model.Email,Email = model.Email,City=model.City};
                var result=await userManager.CreateAsync(RegisteredUser, model.Password);
                if(result.Succeeded)
                {
                    if(signInManager.IsSignedIn(User)&& User.IsInRole("Admin"))

                    {
                        return RedirectToAction("ListUsers", "Adminstration");
                    }
                    await signInManager.SignInAsync(RegisteredUser, isPersistent: false);
                    return RedirectToAction("index","home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
           
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model,string returnUrl)
        {
            if (ModelState.IsValid)
            {
               
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password,model.RememberMe,false);
                if (result.Succeeded)
                {
                    if(!string.IsNullOrEmpty(returnUrl))
                    {
                        return LocalRedirect(returnUrl);
                    }

                    // another way to check local url

                    /* if(!string.IsNullOrEmpty(returnUrl)&&url.IsLocalUrl(returnUrl))
                     *  return Redirect(returnUrl);
                     */
                    else
                    {
                        return RedirectToAction("index", "home");
                    }
                    
                }

           
                    ModelState.AddModelError(string.Empty,"Invalid Login Attempt" );
            
            }
            return View(model);
        }

        [AcceptVerbs("Get","Post")]
        [AllowAnonymous]
        public async Task<IActionResult>IsEmailInUse(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if(user==null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {email} Is Already In Use");
            }
        }

        [HttpPost]
         public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}