using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC__E_Commerce_Project.DAL;
using MVC__E_Commerce_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_commerce_BackFinal.Controllers
{
    public class ProductController : Controller
    {
        private readonly Context _context;


        public ProductController(Context context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Detail(int id)
        {
            Product product = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Brand)
                .Include(p => p.Campaign)
                .Include(p => p.ProductColors)
                .Include(p => p.ProductTags)
                .FirstOrDefaultAsync(p => p.Id == id);
            ViewBag.tags = _context.ProductTags.Include(p => p.Tag).Where(p => p.ProductId == id).ToList();
            return View(product);

        }
    }
}