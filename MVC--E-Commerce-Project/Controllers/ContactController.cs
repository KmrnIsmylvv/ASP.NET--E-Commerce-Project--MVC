using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVC__E_Commerce_Project.DAL;
using MVC__E_Commerce_Project.Models;
using MVC__E_Commerce_Project.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MVC__E_Commerce_Project.Controllers
{
    public class ContactController : Controller
    {
        private readonly Context _context;
        private readonly UserManager<AppUser> _userManager;

        public ContactController(Context context,UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            ContactVM contactVM = new ContactVM();
            string userId = "";

            if (User.Identity.IsAuthenticated)
            {
                userId= HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                contactVM.User = await _userManager.FindByIdAsync(userId);
            }

            Contact contact = _context.Contacts.FirstOrDefault();
            contactVM.Contact = contact;

            return View(contactVM);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromForm]Message message)
        {
            if (User.Identity.IsAuthenticated)
            {
                var dataComment = new Message();

                dataComment.UserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                dataComment.Subject = message.Subject;
                dataComment.Text = message.Text;

                await _context.Messages.AddAsync(dataComment);
                _context.SaveChanges();
            }
            else
            {
                return RedirectToAction("LogIn", "Account");
            }

            return Ok(new { Message = "Success" });
        }
    }
}
