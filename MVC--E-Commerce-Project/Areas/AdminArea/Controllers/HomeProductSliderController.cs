
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC__E_Commerce_Project.DAL;
using MVC__E_Commerce_Project.Extensions;
using MVC__E_Commerce_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Asp.net_E_commerce.Areas.Admin.Controllers
{
    [Area("AdminArea")]
    public class HomeProductSliderController : Controller
    {

        private readonly IWebHostEnvironment _env;
        private readonly Context _context;

        public HomeProductSliderController(Context context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }



        // GET: HomeProductSlidersController
        public ActionResult Index()
        {
            List<HomeProductSlider> slider = _context.HomeProductSliders.Include(x => x.Product).ToList();
            return View(slider);
        }

        // GET: HomeProductSlidersController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: HomeProductSlidersController/Create
        public async Task<ActionResult> Create()
        {
            var products = await _context.Products
             .Include(x => x.Images).ToListAsync();

            ViewBag.ProductList = products;

            return View();
        }

        // POST: HomeProductSlidersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(HomeProductSlider slider)
        {
            if (slider.Title.Length < 15 || slider.Description.Length < 20)
            {
                return View();
            }

            bool isExist = _context.Products.Any(x => x.Id == slider.ProductId);

            if (!isExist)
            {
                return View();
            }


            if (ModelState["Images"].ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
            {
                ModelState.AddModelError("Image", "Do not empty");
                return View();
            }

            if (!slider.Photo.IsImage())
            {
                ModelState.AddModelError("Image", "only image");
                return View();
            }
            if (slider.Photo.IsCorrectSize(300))
            {
                ModelState.AddModelError("Image", "300den yuxari ola bilmez");
                return View();
            }

            HomeProductSlider newSlider = new HomeProductSlider();
            newSlider.Title = slider.Title;
            newSlider.Description = slider.Description;
            newSlider.ProductId = slider.ProductId;
            string fileName = await slider.Photo.SaveImageAsync(_env.WebRootPath, "assets/images/product/");
            newSlider.ImageUrl = fileName;

            await _context.HomeProductSliders.AddAsync(newSlider);
            await _context.SaveChangesAsync();
            var products = await _context.Products
             .Include(x => x.Images).ToListAsync();

            ViewBag.ProductList = products;

            return View();
        }

        // GET: HomeProductSlidersController/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            HomeProductSlider slider = _context.HomeProductSliders.Include(x => x.Product).FirstOrDefault(x => x.Id == id);
            var products = await _context.Products
             .Include(x => x.Images).ToListAsync();

            ViewBag.ProductList = products;
            return View(slider);
        }

        // POST: HomeProductSlidersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(HomeProductSlider slider)
        {
            if (slider.Title.Length < 15 || slider.Description.Length < 20)
            {
                return View();
            }

            bool isExist = _context.Products.Any(x => x.Id == slider.ProductId);

            if (!isExist)
            {
                return View();
            }

            HomeProductSlider newSlider = await _context.HomeProductSliders.FirstOrDefaultAsync(x => x.Id == slider.Id);

            if (newSlider == null) return View();


            newSlider.Title = slider.Title;
            newSlider.Description = slider.Description;
            newSlider.ProductId = slider.ProductId;

            if (slider.Photo != null)
            {
                if (!slider.Photo.IsImage())
                {
                    ModelState.AddModelError("Image", "only image");
                    return View();
                }
                if (slider.Photo.IsCorrectSize(300))
                {
                    ModelState.AddModelError("Image", "300den yuxari ola bilmez");
                    return View();
                }
                string fileName = await slider.Photo.SaveImageAsync(_env.WebRootPath, "assets/images/product/");
                newSlider.ImageUrl = fileName;
            }

            await _context.SaveChangesAsync();


            return View();
        }

        // GET: HomeProductSlidersController/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            HomeProductSlider slider = _context.HomeProductSliders.Include(x => x.Product).FirstOrDefault(x => x.Id == id);
            var products = await _context.Products
             .Include(x => x.Images).ToListAsync();

            ViewBag.ProductList = products;
            return View(slider);
        }

        // POST: HomeProductSlidersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(HomeProductSlider slider)
        {
            HomeProductSlider Deleteslider = _context.HomeProductSliders.Include(x => x.Product).FirstOrDefault(x => x.Id == slider.Id);
            if (Deleteslider == null) return NotFound();

            _context.HomeProductSliders.Remove(Deleteslider);
            await _context.SaveChangesAsync();
            return View();
        }
    }
}