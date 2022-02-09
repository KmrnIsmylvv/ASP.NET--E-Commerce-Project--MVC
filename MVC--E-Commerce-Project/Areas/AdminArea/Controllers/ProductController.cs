
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC__E_Commerce_Project.DAL;
using MVC__E_Commerce_Project.Extensions;
using MVC__E_Commerce_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_commerce_BackFinal.Areas.Admin.Controllers
{
    [Area("AdminArea")]
    public class ProductController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly Context _context;

        public ProductController(Context context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        // GET: ProductController
        public ActionResult Index()
        {
            List<Product> product = _context.Products.Include(c => c.Campaign).Include(b => b.Brand).ToList();
            return View(product);
        }

        // GET: ProductController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult CallCategory(int? id)
        {
            List<Category> categories = _context.BrandCategories.Where(b => b.BrandId == id)
                .Select(c => c.Category).ToList();

            return PartialView("_ProductCreatePartial", categories);
        }

        // GET: ProductController/Create
        public ActionResult Create()
        {

            var brands = new SelectList(_context.Brands.OrderBy(l => l.Name)
            .ToDictionary(us => us.Id, us => us.Name), "Key", "Value");
            ViewBag.Brand = brands;
            var campaign = new SelectList(_context.Campaigns.OrderBy(l => l.Discount)
             .ToDictionary(us => us.Id, us => us.Discount), "Key", "Value");
            ViewBag.Campaign = campaign;
            var colors = _context.Colors.ToList();
            var tags = _context.Tags.ToList();
            ViewBag.Tags = tags;
            ViewBag.Colors = colors;
            return View();
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Product product, int categoryid, int[] tagId, int[] colorId)
        {
            bool isExist = _context.Categories.Any(c => c.Name.ToLower() == product.Name.ToLower().Trim());

            if (isExist)
            {
                ModelState.AddModelError("Name", "This category is already exists");
                return RedirectToAction("Index");
            }
            if (categoryid == 0) return NotFound();

            Product newproduct = new Product()
            {
                Name = product.Name,
                Price = product.Price,
                CampaignId = product.CampaignId,
                BrandId = product.BrandId,
                Availibility = product.Availibility,
                Description = product.Description,
                ExTax = product.ExTax,
                Quantity = product.Quantity,
                IsFeatured = product.IsFeatured,
                ProductCode = product.ProductCode
            };


            await _context.Products.AddAsync(newproduct);
            await _context.SaveChangesAsync();
            ProductRelation productRelation = new ProductRelation()
            {
                ProductId = newproduct.Id,
                BrandId = newproduct.BrandId,
                CategoryId = categoryid
            };
            await _context.ProductRelations.AddAsync(productRelation);
            if (tagId != null && colorId != null)
            {
                foreach (var item in tagId)
                {
                    ProductTag productTag = new ProductTag()
                    {
                        ProductId = newproduct.Id,
                        TagId = item,
                    };
                    await _context.ProductTags.AddAsync(productTag);
                    await _context.SaveChangesAsync();
                }
                foreach (var item in colorId)
                {
                    ProductColor colorProduct = new ProductColor()
                    {
                        ProductId = newproduct.Id,
                        ColorId = item,
                    };
                    await _context.ProductColors.AddAsync(colorProduct);
                    await _context.SaveChangesAsync();
                }
            }

            if (ModelState["Image"].ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
            {
                ModelState.AddModelError("Image", "Do not empty");
            }
            var count = 0;
            foreach (IFormFile photo in product.Photos)
            {
                if (!photo.IsImage())
                {
                    ModelState.AddModelError("Image", "only image");
                    return RedirectToAction("Index");

                }
                if (photo.IsCorrectSize(300))
                {
                    ModelState.AddModelError("Image", "please enter photo under 300kb");
                    return RedirectToAction("Index");
                }
                ProductImages productImage = new ProductImages();

                string fileName = await photo.SaveImageAsync(_env.WebRootPath, "assets/images/product/");
                if (count == 0) productImage.IsMain = true;
                productImage.ImageUrl = fileName;
                productImage.ProductId = newproduct.Id;
                await _context.ProductImages.AddAsync(productImage);
                await _context.SaveChangesAsync();
                count++;
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: ProductController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProductController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProductController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProductController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}