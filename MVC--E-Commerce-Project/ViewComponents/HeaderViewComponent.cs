using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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

namespace MVC__E_Commerce_Project.ViewComponents
{

    public class HeaderViewComponent : ViewComponent
    {
        private readonly Context _context;
        private readonly UserManager<AppUser> _userManager;

        public HeaderViewComponent(Context context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                ViewBag.Username = user.FullName;
            }

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
                HttpContext.Response.Cookies.Append("basketcookie", JsonConvert.SerializeObject(basketProducts), new CookieOptions { MaxAge = TimeSpan.FromDays(14) });

            }
            ViewBag.userid = UserId;
            return View(basketProducts.Where(b => b.UserId == UserId).Take(4).ToList());
        }
    }
}
