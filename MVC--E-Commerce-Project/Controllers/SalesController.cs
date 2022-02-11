
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
    public class SalesController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly Context _context;
        private readonly IConfiguration _config;
        public SalesController(Context context, UserManager<AppUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _context = context;
            _config = config;
        }


        // GET: SalesController
        public async Task<ActionResult> Index()
        {
            string basket = Request.Cookies["basketcookie"];

            if (!User.Identity.IsAuthenticated) return RedirectToAction("index", "home");

            // Get id of the logged in user
            string UserID = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Get baskets product from users cookie

            List<BasketProduct> products = new List<BasketProduct>();

            if (basket != null)
            {

                products = JsonConvert.DeserializeObject<List<BasketProduct>>(basket);

                // Check special basket for logged in user
                var IsExsist = products.FirstOrDefault(x => x.UserId == UserID);
                if (IsExsist == null) return RedirectToAction("index", "home");

                foreach (var item in products)
                {

                    Product product = _context.Products.FirstOrDefault(p => p.Id == item.Id);
                    item.Price = product.Price;

                    item.Name = product.Name;
                }

                Response.Cookies.Append("basketcookie", JsonConvert.SerializeObject(products), new CookieOptions { MaxAge = TimeSpan.FromMinutes(14) });
            }

            ViewBag.User = await _userManager.FindByIdAsync(UserID);

            return View(products.Where(x => x.UserId == UserID).ToList());
        }
        [HttpPost]
        public async Task<ActionResult> Sales(Sales sales)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("index", "home");

            string UserID = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            AppUser _user = await _userManager.FindByIdAsync(UserID);

            Sales _sales = new Sales();
            _sales.SaleDate = DateTime.Now;
            _sales.UserId = _user.Id;


            List<BasketProduct> UserBaket = JsonConvert.DeserializeObject<List<BasketProduct>>(Request.Cookies["basketcookie"]);
            List<BasketProduct> basketProducts = UserBaket.Where(x => x.UserId == UserID).ToList();

            List<SalesProduct> _salesProducts = new List<SalesProduct>();

            List<Product> dbProducts = new List<Product>();

            foreach (var item in basketProducts)
            {
                Product dbProduct = await _context.Products.FindAsync(item.Id);
                if (dbProduct.Quantity < item.Count)
                {
                    return RedirectToAction("Index", "Sales");
                }
                dbProducts.Add(dbProduct);
            }


            double total = 0;
            foreach (var item in basketProducts)
            {
                Product dbProduct = dbProducts.Find(x => x.Id == item.Id);

                await UpdateProductCount(dbProduct, item);

                SalesProduct salesProduct = new SalesProduct();
                salesProduct.SalesId = _sales.Id;
                salesProduct.ProductId = dbProduct.Id;
                _salesProducts.Add(salesProduct);
                total += item.Count * dbProduct.Price;
                UserBaket.Remove(item);
            }
            Response.Cookies.Append("basketcookie", JsonConvert.SerializeObject(UserBaket), new CookieOptions { MaxAge = TimeSpan.FromDays(14) });

            _sales.SalesProducts = _salesProducts;
            _sales.Total = total;

            await _context.Sales.AddAsync(_sales);
            await _context.SaveChangesAsync();


            return RedirectToAction("Index", "Home");
        }

        private async Task UpdateProductCount(Product product, BasketProduct basketProduct)
        {
            product.Quantity = product.Quantity - basketProduct.Count;
            await _context.SaveChangesAsync();
        }
    }
}