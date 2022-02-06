using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVC__E_Commerce_Project.DAL;
using MVC__E_Commerce_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC__E_Commerce_Project.Controllers
{
    public class AboutController : Controller
    {
        private readonly Context _context;

        public AboutController(Context context)
        {
            _context = context;
        }

        // GET: About
        public ActionResult Index()
        {
            About about = _context.Abouts.FirstOrDefault();
            return View(about);
        }

       
    }
}
