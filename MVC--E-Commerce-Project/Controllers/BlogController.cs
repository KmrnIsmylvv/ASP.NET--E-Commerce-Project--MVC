using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC__E_Commerce_Project.DAL;
using MVC__E_Commerce_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC__E_Commerce_Project.Controllers
{
    public class BlogController : Controller
    {
        private readonly Context _context;
        private readonly UserManager<AppUser> _userManager;

        public BlogController(Context context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: BlogController
        public ActionResult Index()
        {
            var blog = _context.Blogs.ToList();
            var photos = _context.BlogPhotos.ToList();
            ViewBag.photos = photos;

            //RECENT POSTS
            var recentPosts = _context.Blogs
                .Take(4)
                .ToList();

            ViewBag.RecentPosts = recentPosts;

            return View(blog);
        }

        // GET: BlogController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            List<Comments> comments = await _context.Comment
                .Where(c => c.BlogId == id)
                .Include(x => x.User).ToListAsync();

            var blog = await _context.Blogs
                .Include(b => b.BlogPhotos)
                .FirstOrDefaultAsync(b => b.Id == id);

            var user = await _userManager.FindByIdAsync(blog.UserId);
            var tags = await _context.ProductTags
                .Where(p => p.ProductId == blog.ProductId)
                .Select(t => t.Tag)
                .ToListAsync();

            var photos = _context.BlogPhotos.ToList();

            //RECENT POSTS
            var recentPosts = _context.Blogs
                .Take(4)
                .ToList();

            // RELATED POSTS
            var product = _context.ProductTags.Include(p => p.Tag).ToList();
            var relatedPosts = _context.Blogs
                .Include(b=>b.BlogPhotos)
                .Include(b => b.Product)
                .ThenInclude(p => p.ProductTags)
                .ThenInclude(r=>r.Tag)
                .ToList();

            ViewBag.relatedPosts = relatedPosts.Where(r => r.Product.ProductTags[0].TagId == product[0].TagId);

            ViewBag.photos = photos;
            ViewBag.RecentPosts = recentPosts;
            ViewBag.user = user.FullName;
            ViewBag.tags = tags;
            ViewBag.comment = comments;

            return View(blog);
        }
    }
}
