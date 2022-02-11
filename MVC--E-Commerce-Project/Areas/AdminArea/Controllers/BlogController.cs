using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC__E_Commerce_Project.DAL;
using MVC__E_Commerce_Project.Extensions;
using MVC__E_Commerce_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MVC__E_Commerce_Project.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    [Authorize(Roles = "Admin")]
    public class BlogController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly Context _context;
        private readonly UserManager<AppUser> _userManager;

        public BlogController(Context context, UserManager<AppUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        // GET: BlogController
        public ActionResult Index()
        {
            List<Blog> blogs = _context.Blogs.Include(b=> b.Product).ToList();
            return View(blogs);
        }

        // GET: BlogController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: BlogController/Create
        public ActionResult Create()
        {
            var products = new SelectList(_context.Products.OrderBy(l => l.Price)
            .ToDictionary(us => us.Id, us => us.Name), "Key", "Value");

            ViewBag.ProductId = products;

            return View();
        }

        // POST: BlogController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Blog blog, string videourl)
        {
            if (blog.Photos == null && videourl == null) return NotFound();

            Blog newBlog = new Blog()
            {
                Title = blog.Title,
                Description = blog.Description,
                ProductId = blog.ProductId,
                Date = blog.Date,
                UserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value,
            };
            await _context.Blogs.AddAsync(newBlog);
            await _context.SaveChangesAsync();

            if (videourl != null)
            {
                BlogPhoto blogPhoto = new BlogPhoto()
                {
                    VideoUrl = videourl,
                    BlogId = newBlog.Id
                };
                await _context.BlogPhotos.AddAsync(blogPhoto);
                await _context.SaveChangesAsync();
            }

            if (blog.Photos != null)
            {
                if (ModelState["Photos"].ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                {
                    ModelState.AddModelError("Image", "Do not empty");
                }

                foreach (IFormFile photo in blog.Photos)
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

                    BlogPhoto blogPhoto = new BlogPhoto();

                    string fileName = await photo.SaveImageAsync(_env.WebRootPath, "assets/images/");

                    blogPhoto.PhotoUrl = fileName;
                    blogPhoto.BlogId = newBlog.Id;

                    await _context.BlogPhotos.AddAsync(blogPhoto);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: BlogController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BlogController/Edit/5
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

        // GET: BlogController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BlogController/Delete/5
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
