using Microsoft.AspNetCore.Mvc;
using MVC__E_Commerce_Project.DAL;
using MVC__E_Commerce_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC__E_Commerce_Project.ViewComponents
{
    public class SubscribeViewComponent:ViewComponent
    {
        private readonly Context _context;

        public SubscribeViewComponent(Context context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            Subscribe subscribe= _context.Subscribes.FirstOrDefault();
            return View(await Task.FromResult(subscribe));
        }
    }
}
