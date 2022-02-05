using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
    public class CompanySliderController : Controller
    {
        private readonly Context _context;
        private readonly IWebHostEnvironment _env;

        public CompanySliderController(Context context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: CompanySliderController
        public ActionResult Index()
        {
            List<CompanySlider> companySliders = _context.CompanySliders.ToList();

            return View(companySliders);
        }

        // GET: CompanySliderController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CompanySliderController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CompanySlider companySlider)
        {
            if (ModelState["Image"].ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
            {
                ModelState.AddModelError("Image", "Do not empty");
            }

            if (!companySlider.Image.IsImage())
            {
                ModelState.AddModelError("Image", "only image");
                return View();
            }
            if (companySlider.Image.IsCorrectSize(300))
            {
                ModelState.AddModelError("Image", "300den yuxari ola bilmez");
                return View();
            }

            CompanySlider newSlider = new CompanySlider();

            string fileName = await companySlider.Image.SaveImageAsync(_env.WebRootPath, "assets/images/brand/");
            newSlider.ImageUrl = fileName;

            await _context.CompanySliders.AddAsync(newSlider);
            await _context.SaveChangesAsync();


            return RedirectToAction("Index");
        }

        // GET: CompanySliderController/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            CompanySlider dbSlider = await _context.CompanySliders.FindAsync(id);

            if (dbSlider == null) return NotFound();
            return View(dbSlider);
        }

        // POST: CompanySliderController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int? id, CompanySlider companySlider)
        {
            if (id == null) return NotFound();

            if (companySlider.Image!= null)
            {
                if (ModelState["Image"].ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                {
                    ModelState.AddModelError("Image", "Do not empty");
                }

                if (!companySlider.Image.IsImage())
                {
                    ModelState.AddModelError("Image", "only image");
                    return View();
                }

                if (companySlider.Image.IsCorrectSize(300))
                {
                    ModelState.AddModelError("Image", "300den yuxari ola bilmez");
                    return View();
                }

                CompanySlider dbSlider = await _context.CompanySliders.FindAsync(id);
                string path = Path.Combine(_env.WebRootPath, "assets/images/brand/", dbSlider.ImageUrl);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                string fileName = await companySlider.Image.SaveImageAsync(_env.WebRootPath, "assets/images/brand/");
                dbSlider.ImageUrl = fileName;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // GET: CompanySliderController/Delete/5
        public async  Task<ActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            CompanySlider dbSlider = await _context.CompanySliders.FindAsync(id);
            
            if (dbSlider == null) return NotFound();
            return View(dbSlider);
        }

        // POST: CompanySliderController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async  Task<ActionResult> DeleteCompanySlider(int? id)
        {
            if (id == null) return NotFound();
            CompanySlider dbSlider = await _context.CompanySliders.FindAsync(id);
            if (dbSlider == null) return NotFound();

            string path = Path.Combine(_env.WebRootPath, "assets/images/brand/", dbSlider.ImageUrl);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            _context.CompanySliders.Remove(dbSlider);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
