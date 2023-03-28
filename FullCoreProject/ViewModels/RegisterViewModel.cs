using FullCoreProject.Utilities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FullCoreProject.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Remote(action:"IsEmailInUse",controller:"Account")]
        [ValidEmailDomain(allowedDomain:"Ahmed.com",ErrorMessage ="Domain Name Must Be Ahmed.com")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name ="ConfirmPassword")]
        [Compare("Password",ErrorMessage ="Password and confirmPassword don't match")]
        public string ConfirmPassword { get; set; }

        public string City { get; set; }
    }
}
