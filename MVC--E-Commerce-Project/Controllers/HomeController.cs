using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            List<Blog> blogs= _context.Blogs.ToList();
            List<Product> products = _context.Products.Include(p => p.Campaign).Include(p => p.Images).Include(p => p.Brand).ToList();

            var photos = _context.BlogPhotos.ToList();
            ViewBag.photos = photos;


            HomeVm homeVm = new HomeVm();
            homeVm.companySliders = companySliders;
            homeVm.services = services;
            homeVm.categories = categories;
            homeVm.Blogs = blogs;
            homeVm.Products = products;

            ViewBag.specialProduct = products.Where(x => x.Name == "S10" && x.Name == "Iphone X");
            

            ViewBag.Featured = products.Where(x => x.IsFeatured == true).OrderByDescending(x => x.Id).Take(8).ToList();

            ViewBag.newArrive = products.OrderByDescending(p => p.Id).Take(14).ToList();

            ViewBag.FeatCategories = categories.Where(c => c.IsFeatured == true);
            
            return View(homeVm);
        }
    }
}
