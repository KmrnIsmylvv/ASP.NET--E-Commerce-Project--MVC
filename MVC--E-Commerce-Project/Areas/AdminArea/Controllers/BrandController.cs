
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC__E_Commerce_Project.DAL;
using MVC__E_Commerce_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_commerce_BackFinal.Areas.Admin.Controllers
{
    [Area("AdminArea")]
    [Authorize(Roles = "Admin")]
    public class BrandController : Controller
    {
        private readonly Context _context;

        public BrandController(Context context)
        {
            _context = context;
        }
        // GET: BrandController
        public ActionResult Index()
        {
            List<Brand> brands = _context.Brands.Include(b => b.BrandCategories).ThenInclude(c => c.Category).ToList();
            return View(brands);
        }

        // GET: BrandController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: BrandController/Create
        public ActionResult Create()
        {
            ViewBag.IsMainCategory = _context.Categories.Where(c => c.IsMain == true).ToList();
            ViewBag.SubCategory = _context.Categories.Where(c => c.IsMain == false).ToList();

            return View();
        }

        // POST: BrandController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Brand brand, int[] subcategory)
        {
            bool isExist = _context.Brands.Any(c => c.Name.ToLower() == brand.Name.ToLower().Trim());

            if (isExist)
            {
                ModelState.AddModelError("Name", "This category is already exists");
                return RedirectToAction("Index");

            }


            Brand newBrand = new Brand();
            newBrand.Name = brand.Name;
            await _context.Brands.AddAsync(newBrand);
            await _context.SaveChangesAsync();


            if (subcategory != null)
            {

                foreach (var item in subcategory)
                {
                    BrandCategory categoryBrands = new BrandCategory();
                    categoryBrands.BrandId = newBrand.Id;
                    categoryBrands.CategoryId = item;
                    await _context.AddAsync(categoryBrands);
                    await _context.SaveChangesAsync();
                }

            }
            return RedirectToAction(nameof(Index));
        }

        // GET: BrandController/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            Brand brand = await _context.Brands.Include(b => b.BrandCategories).ThenInclude(c => c.Category).FirstOrDefaultAsync(c => c.Id == id);
            List<BrandCategory> SubCategory = await _context.BrandCategories.Include(c => c.Category).Where(x => x.BrandId == brand.Id).ToListAsync();

            List<Category> AllCategory = await _context.Categories.Include(c => c.BrandCategories).ThenInclude(c => c.Brand).Where(c => c.IsMain == false).ToListAsync();
            foreach (var item in SubCategory)
            {
                AllCategory.Remove(item.Category);

            }
            ViewBag.checkCategory = SubCategory;
            ViewBag.noneCheck = AllCategory;
            return View(brand);
        }

        // POST: BrandController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int? id, Brand brand, List<int> subcategory)
        {
            bool isExist = _context.Brands.Any(c => c.Name.ToLower() == brand.Name.ToLower().Trim());
            Brand newBrand = await _context.Brands.FindAsync(id);

            if (isExist && !(newBrand.Name.ToLower() == brand.Name.ToLower().Trim()))
            {
                ModelState.AddModelError("Name", $"{newBrand} brand already exists");
                return RedirectToAction("Edit");
            }

            if (subcategory.Count() == 0)
            {
                ModelState.AddModelError("Name", "Must choose at least 1 category");
                return RedirectToAction("Edit");
            }

            List<int> checkedCategory = _context.BrandCategories.Where(c => c.BrandId == newBrand.Id).Select(i => i.CategoryId).ToList();

            List<int> addedCategory = subcategory.Except(checkedCategory).ToList();
            List<int> removedCategory = checkedCategory.Except(subcategory).ToList();

            int addedCategoryLength = addedCategory.Count();
            int removedCategoryLength = removedCategory.Count();
            int FullLength = addedCategoryLength + removedCategoryLength;

            newBrand.Name = brand.Name;

            for (int i = 1; i <= FullLength; i++)
            {
                if (addedCategoryLength >= i)
                {
                    BrandCategory categoryBrand = new BrandCategory();
                    categoryBrand.BrandId = newBrand.Id;
                    categoryBrand.CategoryId = addedCategory[i - 1];
                    await _context.BrandCategories.AddAsync(categoryBrand);
                    await _context.SaveChangesAsync();
                }

                if (removedCategoryLength >= i)
                {
                    BrandCategory categoryBrand = await _context.BrandCategories.FirstOrDefaultAsync(c => c.CategoryId == removedCategory[i - 1] && c.BrandId == newBrand.Id);
                    _context.BrandCategories.Remove(categoryBrand);
                    await _context.SaveChangesAsync();
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: BrandController/Delete/5
        public async  Task<ActionResult> Delete(int id)
        {
            Brand _brand = await _context.Brands.FirstOrDefaultAsync(x => x.Id == id);
            if (_brand == null) return RedirectToAction("Delete");
            return View(_brand);
        }

        // POST: BrandController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Brand brand)
        {
            Brand _brand = await _context.Brands.FirstOrDefaultAsync(x => x.Id == brand.Id);
            if (brand == null) return RedirectToAction("Delete");


            List<BrandCategory> categoryBrand = await _context.BrandCategories.ToListAsync();
            foreach (var item in categoryBrand)
            {

                _context.BrandCategories.Remove(item);
                await _context.SaveChangesAsync();


            }
            _context.Brands.Remove(_brand);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}