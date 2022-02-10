using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MVC__E_Commerce_Project.DAL;
using MVC__E_Commerce_Project.Extensions;
using MVC__E_Commerce_Project.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MVC__E_Commerce_Project.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    [Authorize(Roles = "Admin")]
    public class ServiceController : Controller
    {
        private readonly Context _context;
        private readonly IWebHostEnvironment _env;

        public ServiceController(Context context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            List<Service> services = _context.Services.ToList();
            return View(services);
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            Service dbService = await _context.Services.FindAsync(id);

            if (dbService == null) return NotFound();
            return View(dbService);
        }

        // GET: CompanySliderController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CompanySliderController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Service service)
        {
            bool isExistService = _context.Services.Any(s => s.Title.ToLower().Trim() == service.Title.ToLower().Trim());
            if (isExistService)
            {
                ModelState.AddModelError("Title", "The service with this title already exists");
                View();
            }

            if (ModelState["Image"].ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
            {
                ModelState.AddModelError("Image", "Do not empty");
            }

            if (!service.Image.IsImage())
            {
                ModelState.AddModelError("Image", "only image");
                return View();
            }

            if (service.Image.IsCorrectSize(300))
            {
                ModelState.AddModelError("Image", "300den yuxari ola bilmez");
                return View();
            }

            Service newService = new Service();

            string fileName = await service.Image.SaveImageAsync(_env.WebRootPath, "assets/images/banner-icon/");
            newService.ImageUrl = fileName;
            newService.Description = service.Description;
            newService.Title = service.Title;

            await _context.Services.AddAsync(newService);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET: CompanySliderController/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            Service dbService = await _context.Services.FindAsync(id);

            if (dbService== null) return NotFound();
            return View(dbService);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Service services)
        {
            if (id == null) return NotFound();

            if (!ModelState.IsValid) return View();

            bool isExist = _context.Services.Any(s => s.Title.ToLower().Trim() == services.Title.ToLower().Trim());

           Service isExistService = _context.Services.FirstOrDefault(s => s.Id == services.Id);

            if (isExist && !(isExistService.Title.ToLower() == services.Title.ToLower().Trim()))
            {
                ModelState.AddModelError("Title", "The service with this title already exists");
                View();
            };



            if (services.Image != null)
            {
                if (ModelState["Image"].ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                {
                    ModelState.AddModelError("Image", "Please, don't empty");
                }

                if (!services.Image.IsImage())
                {
                    ModelState.AddModelError("Photo", "just image");
                    return View();
                }
                if (services.Image.IsCorrectSize(400))
                {
                    ModelState.AddModelError("Photo", "Enter the size correctly");
                    return View();
                }
                Service dbServices = await _context.Services.FindAsync(id);
                string path = Path.Combine(_env.WebRootPath, "assets/images/banner-icon/", dbServices.ImageUrl);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                string fileName = await services.Image.SaveImageAsync(_env.WebRootPath, "assets/images/banner-icon/");


                dbServices.ImageUrl = fileName;
                dbServices.Title = services.Title;
                dbServices.Description = services.Description;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            Service dbServices = await _context.Services.FindAsync(id);

            if (dbServices == null) return NotFound();
            return View(dbServices);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]

        public async Task<IActionResult> DeleteServices(int? id)
        {
            if (id == null) return NotFound();
            Service dbServices = await _context.Services.FindAsync(id);

            if (dbServices == null) return NotFound();

            string path = Path.Combine(_env.WebRootPath, "assets/images/banner-icon/", dbServices.ImageUrl);

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            _context.Services.Remove(dbServices);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
