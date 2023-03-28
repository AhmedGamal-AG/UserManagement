using FullCoreProject.Models;
using FullCoreProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FullCoreProject.Controllers
{

    //[Route("Home")]
    //[Route("[controller]")]  //token replacement
    // [Route("[controller]/[action]")]  //token replacement

    

    public class HomeController:Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IHostingEnvironment hostingEnvironment;

        public HomeController(IEmployeeRepository employeeRepository,IHostingEnvironment hostingEnvironment)
        {
            _employeeRepository = employeeRepository;
            this.hostingEnvironment = hostingEnvironment;
        }
     //   [Route("~/Home")] //with token     [Route("[controller]/[action]")]  //token replacement
     //   [Route("")]
    //[Route("[action]")]
    // [Route("Index")]
    //[Route("/")]//root as default  when controller is mandatory
    //  [Route("~/")]//root as default when controller is mandatory
    //[Route("Home")]
    //[Route("Home/Index")]
    // [Route("Home/List")]

    [AllowAnonymous]
    public ViewResult Index()
        {
            // return _employeeRepository.GetEmployee(1).Name;
            //return View("~/Views/Home/Index.cshtml",_employeeRepository.GetAllEmployees());
            return View(_employeeRepository.GetAllEmployees());
        }
        //public ObjectResult Details(int id)
        //{
        //    Employee model = _employeeRepository.GetEmployee(id);
        //    return new ObjectResult(model);
        //}

        //[Route("Home/Details/{id?}")]
        public ViewResult Details(int? id)
        {
            throw new Exception("Error in details view");
            var employee= _employeeRepository.GetEmployee(id.Value);
            if(employee==null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound",id.Value);
            }
            Employee model = employee;
            return View(model);
            // return View("Test"); change view default behaviour
            //  return View("MyViews/TestView.cshtml"); //change view default behaviour
        }

        //public ViewResult Details()
        //{
        //    HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
        //    {
        //        Employee = _employeeRepository.GetEmployee(2),
        //        PageTitle = "Employee View Model"

        //    };
        //    //Employee model = _employeeRepository.GetEmployee(2);
        //    ////ViewData["Employee"] = model;
        //    ////return View("Test");
        //    //ViewBag.employee = model;
        //    return View(homeDetailsViewModel);

        //}

        [HttpGet]
     
        public ViewResult Create()
        {
            return View();
        }

        [HttpPost]
       
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            if(ModelState.IsValid)
            {
                string UniqueFileName = null;
                if(model.Photo!=null)
                {
                   string uploadsFolder= Path.Combine(hostingEnvironment.WebRootPath, "Images");
                   UniqueFileName= Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                   string filepath = Path.Combine(uploadsFolder, UniqueFileName);
                    using ( var filestream= new FileStream(filepath, FileMode.Create))
                    {
                        model.Photo.CopyTo(filestream);
                    }
                        

                }
                var NewEmployee = new Employee { Name = model.Name, Email = model.Email, Department = model.Department, PhotoPath = UniqueFileName};
                _employeeRepository.Add(NewEmployee);
                 
                 return RedirectToAction("Index");
               
            }
            return View();
        }
        [HttpGet]
        
        public ViewResult Edit(int id)
        {
            var employee = _employeeRepository.GetEmployee(id);
            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel { id = employee.Id, Name = employee.Name, Email = employee.Email, Department = employee.Department, ExistingPhotoPath = employee.PhotoPath };
            return View(employeeEditViewModel);
        }

  

        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var employee = _employeeRepository.GetEmployee(model.id);
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;
                

                string UniqueFileName = null;
                if (model.Photo != null)
                { 
                    if(model.ExistingPhotoPath!=null)
                    {
                        string Deletedfilepath = Path.Combine(hostingEnvironment.WebRootPath, "Images", model.ExistingPhotoPath);
                        System.IO.File.Delete(Deletedfilepath);
                    }
                    string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "Images");
                    UniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                    string filepath = Path.Combine(uploadsFolder, UniqueFileName);
                    employee.PhotoPath = UniqueFileName;
                    using (var filestream = new FileStream(filepath, FileMode.Create))
                    {
                        model.Photo.CopyTo(filestream);
                    }
                   
                }
               // var NewEmployee = new Employee {  Name = model.Name, Email = model.Email, Department = model.Department, PhotoPath = UniqueFileName };
                _employeeRepository.Update(employee);

                return RedirectToAction("Index");

            }
            return View();
        }

    }
}
