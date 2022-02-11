
using Microsoft.AspNetCore.Authorization;
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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace E_commerce_BackFinal.Areas.Admin.Controllers
{
    [Area("AdminArea")]
    [Authorize(Roles = "Admin")]
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
        //public ActionResult Details(int? id)
        //{
        //    if (id == null) return NotFound();
            
        //    Product product
        
        //}

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

            if (ModelState["Photos"].ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
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

            return RedirectToAction("Index");
        }

        // GET: ProductController/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            Product product = await _context.Products.FindAsync(id);
            var relation = _context.ProductRelations.Where(b => b.ProductId == id && b.BrandId == product.BrandId).FirstOrDefault();
            Category category = await _context.Categories.FindAsync(relation.CategoryId);
            ViewBag.category = category;

            var brands = new SelectList(_context.Brands.OrderBy(l => l.Name)
            .ToDictionary(us => us.Id, us => us.Name), "Key", "Value");
            ViewBag.BrandId = brands;

            var campaign = new SelectList(_context.Campaigns.OrderBy(l => l.Discount)
             .ToDictionary(us => us.Id, us => us.Discount), "Key", "Value");
            ViewBag.CampaignId = campaign;

            var photos = _context.ProductImages.Where(p => p.ProductId == id).ToList();
            ViewBag.photos = photos;

            var checkTag = await _context.ProductTags.Where(p => p.ProductId == id).Select(t => t.Tag).ToListAsync();
            var checkColor = await _context.ProductColors.Where(p => p.ProductId == id).Select(c => c.Color).ToListAsync();
            ViewBag.checkTag = checkTag;
            ViewBag.checkColor = checkColor;

            var allTag = await _context.Tags.ToListAsync();
            var allColor = await _context.Colors.ToListAsync();

            var noneCheckTag = allTag.Except(checkTag);
            var noneCheckColor = allColor.Except(checkColor);
            ViewBag.noneTag = noneCheckTag;
            ViewBag.noneColor = noneCheckColor;

            return View(product);
        }

        // POST: ProductController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int? id, Product product, int categoryid, List<int> tagId, List<int> colorId)
        {
            if (categoryid == null) return RedirectToAction("Edit", "Product");

            Product newProduct = await _context.Products.FindAsync(id);
            var relationProduct = _context.ProductRelations.Where(p => p.ProductId == id && p.BrandId == newProduct.BrandId).FirstOrDefault();

            newProduct.Name = product.Name;
            newProduct.Price = product.Price;
            newProduct.CampaignId = product.CampaignId;
            newProduct.BrandId = product.BrandId;
            newProduct.Availibility = product.Availibility;
            newProduct.Description = product.Description;
            newProduct.ExTax = product.ExTax;
            newProduct.Quantity = product.Quantity;
            newProduct.IsFeatured = product.IsFeatured;
            newProduct.ProductCode = product.ProductCode;
            await _context.SaveChangesAsync();

            if (relationProduct.CategoryId != categoryid || relationProduct.BrandId != newProduct.BrandId)
            {
                _context.ProductRelations.Remove(relationProduct);
                ProductRelation newProductRelation = new ProductRelation();
                newProductRelation.CategoryId = categoryid;
                newProductRelation.BrandId = newProduct.BrandId;
                newProductRelation.ProductId = newProduct.Id;
                await _context.ProductRelations.AddAsync(newProductRelation);
                await _context.SaveChangesAsync();
            }

            List<int> checkTag = _context.ProductTags.Where(p => p.ProductId == newProduct.Id).Select(t => t.TagId).ToList();
            List<int> checkColor = _context.ProductColors.Where(c => c.ProductId == newProduct.Id).Select(c => c.ColorId).ToList();

            List<int> addedTag = tagId.Except(checkTag).ToList();
            List<int> removeTag = checkTag.Except(tagId).ToList();

            List<int> addedColor = colorId.Except(checkColor).ToList();
            List<int> removeColor = checkColor.Except(colorId).ToList();

            int addedTagLength = addedTag.Count();
            int removedTagLength = removeTag.Count();
            int FullLength = addedTagLength + removedTagLength;

            int addedColorLength = addedColor.Count();
            int removedColorLength = removeColor.Count();
            int FullLengthColor = addedColorLength + removedColorLength;



            for (int i = 1; i <= FullLength; i++)
            {
                if (addedTagLength >= i)
                {
                    ProductTag productTag = new ProductTag();
                    productTag.ProductId = newProduct.Id;
                    productTag.TagId = addedTag[i - 1];
                    await _context.ProductTags.AddAsync(productTag);
                    await _context.SaveChangesAsync();
                }

                if (removedTagLength >= i)
                {
                    ProductTag productTag = await _context.ProductTags.FirstOrDefaultAsync(c => c.TagId == removeTag[i - 1] && c.ProductId == newProduct.Id);
                    _context.ProductTags.Remove(productTag);
                    await _context.SaveChangesAsync();
                }
            }

            for (int i = 1; i <= FullLengthColor; i++)
            {
                if (addedTagLength >= i)
                {
                    ProductColor colorProduct = new ProductColor();
                    colorProduct.ProductId = newProduct.Id;
                    colorProduct.ColorId = addedColor[i - 1];
                    await _context.ProductColors.AddAsync(colorProduct);
                    await _context.SaveChangesAsync();
                }

                if (removedTagLength >= i)
                {
                    ProductColor colorProduct = await _context.ProductColors.FirstOrDefaultAsync(c => c.ColorId == removeColor[i - 1] && c.ProductId == newProduct.Id);
                    _context.ProductColors.Remove(colorProduct);
                    await _context.SaveChangesAsync();
                }
            }

            if (product.Photos != null)
            {
                if (ModelState["Photos"].ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                {
                    ModelState.AddModelError("Image", "Do not empty");
                }
                var count = 0;
                var oldPhoto = _context.ProductImages.Where(p => p.ProductId == newProduct.Id).ToList();


                if (oldPhoto.Count <= product.Photos.Length)
                {
                    foreach (var item in oldPhoto)
                    {
                        string path = Path.Combine(_env.WebRootPath, "assets/images/product/", item.ImageUrl);
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        _context.ProductImages.Remove(item);
                        await _context.SaveChangesAsync();
                    }
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



                        ProductImages productPhoto = new ProductImages();

                        string fileName = await photo.SaveImageAsync(_env.WebRootPath, "assets/images/product/");

                        if (count == 0) productPhoto.IsMain = true;

                        productPhoto.ImageUrl = fileName;
                        productPhoto.ProductId = newProduct.Id;

                        await _context.ProductImages.AddAsync(productPhoto);
                        await _context.SaveChangesAsync();
                        count++;
                    }
                }
                else
                {
                    for (int i = 0; i < product.Photos.Length; i++)
                    {
                        if (!product.Photos[i].IsImage())
                        {
                            ModelState.AddModelError("Image", "only image");
                            return RedirectToAction("Index");

                        }
                        if (product.Photos[i].IsCorrectSize(300))
                        {
                            ModelState.AddModelError("Image", "please enter photo under 300kb");
                            return RedirectToAction("Index");
                        }
                        string fileName = await product.Photos[i].SaveImageAsync(_env.WebRootPath, "assets/images/product/");
                        string path = Path.Combine(_env.WebRootPath, "assets/images/product/", oldPhoto[i].ImageUrl);
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        oldPhoto[i].ImageUrl = fileName;
                        await _context.SaveChangesAsync();
                    }
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET: ProductController/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            Product product = await _context.Products
                .Include(p => p.Campaign)
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(p => p.Id == id);

            var relation = await _context.ProductRelations
                .Where(p => p.ProductId == id && p.BrandId == product.BrandId)
                .FirstOrDefaultAsync();

            Category category = await _context.Categories.FindAsync(relation.CategoryId);
            ViewBag.category = category;

            ViewBag.photo = _context.ProductImages
               .Where(p => p.ProductId == product.Id && p.IsMain == true)
               .FirstOrDefault();

            ViewBag.color = await _context.ProductColors
                .Where(p => p.ProductId == id)
                .Select(c => c.Color)
                .ToListAsync();

            return View(product);
        }

        //POST: ProductController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, Product product)
        {
            var currentProduct = await _context.Products.FindAsync(id);
            if (currentProduct == null) return NotFound();

            var productTags = await _context.ProductTags.Where(t => t.ProductId == id).ToListAsync();
            if (productTags != null)
            {
                foreach (var item in productTags)
                {
                    _context.ProductTags.Remove(item);
                    await _context.SaveChangesAsync();
                }
            }

            var productColors = await _context.ProductColors
                .Where(c => c.ProductId == id)
                .ToListAsync();

            if (productColors != null)
            {
                foreach (var item in productColors)
                {
                    _context.ProductColors.Remove(item);
                    await _context.SaveChangesAsync();
                }
            }

            var relationProduct = await _context.ProductRelations
                .FirstOrDefaultAsync(p => p.ProductId == id && p.BrandId == currentProduct.BrandId);
            _context.ProductRelations.Remove(relationProduct);

            if (currentProduct.Photos != null)
            {
                var oldPhoto = _context.ProductImages
                    .Where(p => p.ProductId == currentProduct.Id)
                    .ToList();

                foreach (var item in oldPhoto)
                {
                    string path = Path.Combine(_env.WebRootPath, "assets/images/product/", item.ImageUrl);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    _context.ProductImages.Remove(item);
                    await _context.SaveChangesAsync();
                }
            }

            _context.Products.Remove(currentProduct);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
    }
}