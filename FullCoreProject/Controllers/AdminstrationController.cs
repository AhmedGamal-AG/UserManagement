using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FullCoreProject.Models;
using FullCoreProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FullCoreProject.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminstrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> useManager;

        public AdminstrationController(RoleManager<IdentityRole>roleManager,UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.useManager = userManager;
        }
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if(ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole { Name = model.RoleName };
                var result = await roleManager.CreateAsync(identityRole);
                if(result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Adminstration");
                }
                foreach(IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = useManager.Users;
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
          var role=  await roleManager.FindByIdAsync(id);
            if(role==null)
            {
                ViewBag.ErrorMessage = $"Role With Id={id} cannot be found";
                return View("NotFound");
            }
            var model = new EditRoleViewModel { Id = role.Id,RoleName=role.Name };

            foreach(var user in useManager.Users)
            {
                if(await useManager.IsInRoleAsync(user,role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Policy ="EditRolePolicy")]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role With Id={model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);
                if(result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }
            
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
           
       
          
        }

        [HttpGet]
        public async Task<IActionResult>EditUserInRole(string roleId)
        {
            ViewBag.roleId = roleId;
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role With Id={roleId} cannot be found";
                return View("~/Views/Error/NotFound.cshtml");
            }

            var model = new List<UserRoleViewModel>();

            foreach( var user in useManager.Users)
            {
                var userRoleViewModel = new UserRoleViewModel { Id = user.Id, UserName = user.UserName };

                if(await useManager.IsInRoleAsync(user,role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);
            }

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> EditUserInRole(List<UserRoleViewModel>model,string roleId)
        {
            ViewBag.roleId = roleId;
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role With Id={roleId} cannot be found";
                return View("~/Views/Error/NotFound.cshtml");
            }

            for(int i=0;i<model.Count;i++)
            {
                var user = await useManager.FindByIdAsync(model[i].Id);
                IdentityResult result = null;
                if(model[i].IsSelected &&!(await useManager.IsInRoleAsync(user,role.Name)) )
                {
                    result= await useManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await useManager.IsInRoleAsync(user, role.Name))
                {
                    result=await useManager.RemoveFromRoleAsync(user,role.Name);
                }
                else
                {
                    continue;
                }

                if(result.Succeeded)
                {
                    if (i < model.Count - 1)
                        continue;
                    else
                        return RedirectToAction("EditRole", new { id = roleId });
                }


            }

            return RedirectToAction("EditRole", new { id = roleId });

   
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await useManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"Role With Id={id} cannot be found";
                return View("NotFound");
            }

            var userClaims = await useManager.GetClaimsAsync(user);
            var userRoles = await useManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {   Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                City = user.City,
                Claims = userClaims.Select(c => c.Type+":"+c.Value).ToList(),
                Roles=userRoles

            };


            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            ViewBag.userId = userId;
            var user = await useManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"Role With Id={userId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRolesViewModel>();

            foreach(var role in roleManager.Roles)
            {
                UserRolesViewModel userRolesViewModel = new UserRolesViewModel { RoleId = role.Id, RoleName = role.Name };

                if (await useManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.IsSelected = true;
                }
                else
                {
                    userRolesViewModel.IsSelected = false;
                }

                model.Add(userRolesViewModel);

            }


            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> model,string userId)
        {
            
            var user = await useManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"Role With Id={userId} cannot be found";
                return View("NotFound");
            }

            var roles = await useManager.GetRolesAsync(user);
            var result = await useManager.RemoveFromRolesAsync(user, roles);

           if(!result.Succeeded)
            {
                ModelState.AddModelError("", "Can not remove user existing roles");
                return View(model);
            }

            result = await useManager.AddToRolesAsync(user, model.Where(x => x.IsSelected).Select(x => x.RoleName));
          if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Can add selected roles to user ");
                return View(model);
            }
            return RedirectToAction("EditUser", new { Id = userId });

           
        }


        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await useManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"Role With Id={model.Id} cannot be found";
                return View("NotFound");
            }

            else
            {
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.City = model.City;

                IdentityResult result = await useManager.UpdateAsync(user);

                if(result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }


            
        }



        [HttpPost]
        public async Task<IActionResult> DeleteUser(string Id)
        {
            var user = await useManager.FindByIdAsync(Id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"Role With Id={Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                var result = await useManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View("ListUsers");

            }

        }


        [HttpPost]
        [Authorize(Policy="DeleteRolePolicy")]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role With Id={roleId} cannot be found";
                return View("NotFound");
            }
            else
            {
                var result = await roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View("ListRoles");

            }

        }



        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            ViewBag.userId = userId;
            var user = await useManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"Role With Id={userId} cannot be found";
                return View("NotFound");
            }

            var existingUserClaims = await useManager.GetClaimsAsync(user);

            var model = new UserClaimsViewModel()
            {
                UserId = userId
            };

            // Loop through each claim we have in our application
            foreach (Claim claim in Models.ClaimStore.AllClaims)
            {
                UserClaim userClaim = new UserClaim
                {
                    claimType = claim.Type
                };

                // If the user has the claim, set IsSelected property to true, so the checkbox
                // next to the claim is checked on the UI
                if (existingUserClaims.Any(c => c.Type == claim.Type && c.Value=="true"))
                {
                    userClaim.IsSelected = true;
                }

                model.Claims.Add(userClaim);
            }
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model)
        {
            var user = await useManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.UserId} cannot be found";
                return View("NotFound");
            }

            // Get all the user existing claims and delete them
            var claims = await useManager.GetClaimsAsync(user);
            var result = await useManager.RemoveClaimsAsync(user, claims);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing claims");
                return View(model);
            }

            // Add all the claims that are selected on the UI
            result = await useManager.AddClaimsAsync(user,
               //  model.Claims.Where(c => c.IsSelected).Select(c => new Claim(c.claimType, c.claimType)));
               model.Claims.Select(c => new Claim(c.claimType, c.IsSelected ? "true":"false")));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected claims to user");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = model.UserId });

        }



    }
}