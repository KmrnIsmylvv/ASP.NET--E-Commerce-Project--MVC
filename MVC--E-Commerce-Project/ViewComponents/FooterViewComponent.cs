using Microsoft.AspNetCore.Mvc;
using MVC__E_Commerce_Project.DAL;
using MVC__E_Commerce_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC__E_Commerce_Project.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly Context _context;

        public FooterViewComponent(Context context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            Contact contact = _context.Contacts.FirstOrDefault();
            return View(await Task.FromResult(contact));
        }
    }
}
