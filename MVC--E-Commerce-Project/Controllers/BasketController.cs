using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC__E_Commerce_Project.DAL;
using MVC__E_Commerce_Project.Models;
using MVC__E_Commerce_Project.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace E_commerce_BackFinal.Controllers
{
    public class BasketController : Controller
    {
        private readonly Context _context;
        public BasketController(Context context)
        {
            _context = context;
        }
        public async Task<IActionResult> AddBasket(int id)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Account");

            if (id == null) return RedirectToAction("Index", "Home");

            Product product = await _context.Products.Include(p => p.Campaign).Include(p => p.Brand)
                .Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            string basket = Request.Cookies["basketcookie"];
            List<BasketProduct> basketProducts;

            if (basket == null) basketProducts = new List<BasketProduct>();
            else basketProducts = JsonConvert.DeserializeObject<List<BasketProduct>>(basket);

            BasketProduct isExsistProduct = basketProducts.FirstOrDefault(p => p.Id == product.Id);
            if (isExsistProduct == null)
            {
                BasketProduct basketProduct = new BasketProduct
                {
                    Id = product.Id,
                    Name = product.Name,
                    UserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value,
                    Count = 1,
                    BrandId = product.BrandId,
                    Discount = product.Campaign.Discount,
                    PhotoUrl = product.Images[0].ImageUrl,
                    Price = product.Price
                };
                basketProducts.Add(basketProduct);
            }
            else isExsistProduct.Count++;

            Response.Cookies.Append("basketcookie", JsonConvert.SerializeObject(basketProducts), new CookieOptions { MaxAge = TimeSpan.FromDays(14) });
            return RedirectToAction("Index", "Home");

        }

        public async Task<IActionResult> ShowBasket()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Account");
            var UserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            string basket = Request.Cookies["basketcookie"];

            List<BasketProduct> basketProducts = new List<BasketProduct>();
            if (basket != null)
            {
                basketProducts = JsonConvert.DeserializeObject<List<BasketProduct>>(basket);
                foreach (var item in basketProducts)
                {
                    Product product = await _context.Products.Include(p => p.Campaign)
                        .Include(p => p.ProductColors)
                        .Include(p => p.Brand)
                        .Include(p => p.Images)
                        .FirstOrDefaultAsync(p => p.Id == item.Id);
                    item.Price = product.Price;
                    item.PhotoUrl = product.Images[0].ImageUrl;
                    item.Name = product.Name;
                    item.Discount = product.Campaign.Discount;
                }
                Response.Cookies.Append("basketcookie", JsonConvert.SerializeObject(basketProducts), new CookieOptions { MaxAge = TimeSpan.FromDays(14) });

            }
            ViewBag.userid = UserId;
            return View(basketProducts);
        }

        public IActionResult BasketCount([FromForm] int id, string change)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Account");
            var UserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            string basket = Request.Cookies["basketcookie"];
            List<BasketProduct> basketProducts = new List<BasketProduct>();
            basketProducts = JsonConvert.DeserializeObject<List<BasketProduct>>(basket);
            Product product = _context.Products.Find(id);
            var totalcount = 0;
            foreach (var item in basketProducts)
            {
                if (item.Id == id && item.UserId == UserId)
                {
                    if (change == "sub" && (item.Count) > 1)
                    {
                        item.Count--;
                        totalcount += item.Count;

                    }
                    if (change == "add" && item.Count != product.Quantity)
                    {
                        item.Count++;
                        totalcount += item.Count;
                    }
                    if (totalcount != 0) item.Count = totalcount;
                }

            }

            Response.Cookies.Append("basketcookie", JsonConvert.SerializeObject(basketProducts), new CookieOptions { MaxAge = TimeSpan.FromDays(14) });
            if (totalcount != 0)
            {
                return Ok(totalcount);
            }
            return Ok("error");
        }

        public IActionResult BasketRemove(int id)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Account");
            var UserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            string basket = Request.Cookies["basketcookie"];
            List<BasketProduct> basketProducts = new List<BasketProduct>();

            basketProducts = JsonConvert.DeserializeObject<List<BasketProduct>>(basket);
            Product product = _context.Products.Find(id);
            foreach (var item in basketProducts)
            {
                if (item.Id == id && item.UserId == UserId)
                {
                    basketProducts.Remove(item);
                    break;
                }

            }
            Response.Cookies.Append("basketcookie", JsonConvert.SerializeObject(basketProducts), new CookieOptions { MaxAge = TimeSpan.FromDays(14) });
            return Ok();
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}