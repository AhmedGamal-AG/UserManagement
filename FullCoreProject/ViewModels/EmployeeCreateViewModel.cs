using FullCoreProject.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FullCoreProject.ViewModels
{
    public class EmployeeCreateViewModel
    {
       

        [Required]
        [MaxLength(50, ErrorMessage = "Name Cannot exceed 50 characters")]
        public string Name { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$",
        ErrorMessage = "Invalid email format")]
        [Display(Name = "Office Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please Select Department")]
        public Dept Department { get; set; }

          public IFormFile Photo { get; set; }

        //public List<IFormFile> Photos { get; set; }
    }
}
