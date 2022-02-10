using Microsoft.AspNetCore.Mvc;
using MVC__E_Commerce_Project.DAL;
using MVC__E_Commerce_Project.Models;
using MVC__E_Commerce_Project.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC__E_Commerce_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly Context _context;
        public HomeController(Context context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<CompanySlider> companySliders = _context.CompanySliders.ToList();
            List<Service> services = _context.Services.ToList();
            List<Category> categories = _context.Categories.Where(c => c.IsMain == true).ToList();

            HomeVm homeVm = new HomeVm();
            homeVm.companySliders = companySliders;
            homeVm.services = services;
            homeVm.categories = categories;

            ViewBag.FeatCategories = categories.Where(c => c.IsFeatured == true);
            
            return View(homeVm);
        }
    }
}
