using Microsoft.AspNetCore.Mvc;
using MVC__E_Commerce_Project.DAL;
using MVC__E_Commerce_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC__E_Commerce_Project.Controllers
{
    public class ContactController : Controller
    {
        private readonly Context _context;

        public ContactController(Context context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            Contact contact = _context.Contacts.FirstOrDefault();
            return View(contact);
        }
    }
}
