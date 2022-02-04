using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVC__E_Commerce_Project.DAL;
using MVC__E_Commerce_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC__E_Commerce_Project.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class ContactController : Controller
    {
        private readonly Context _context;

        public ContactController(Context context)
        {
            _context = context;
        }
        // GET: ContactController
        public ActionResult Index()
        {
            List<Contact> contacts = _context.Contacts.ToList();
            return View(contacts);
        }

        // GET: ContactController/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            Contact contact = await _context.Contacts.FindAsync(id);

            if (contact == null) return NotFound();

            return View(contact);
        }

        // GET: ContactController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ContactController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Contact contact)
        {
            Contact newContact = new Contact
            {
                Description = contact.Description,
                Address = contact.Address,
                Phone = contact.Phone,
                Email = contact.Email
            };

            await _context.AddAsync(newContact);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Contact");
        }

        // GET: ContactController/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            Contact contact = await _context.Contacts.FindAsync(id);
            if (contact == null) return NotFound();
            return View(contact);
        }

        // POST: ContactController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Contact contact)
        {
            Contact dbContact = _context.Contacts.FirstOrDefault(c => c.Id == contact.Id);

            dbContact.Description = contact.Description;
            dbContact.Address = contact.Address;
            dbContact.Phone = contact.Phone;
            dbContact.Email = contact.Email;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: ContactController/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            Contact contact = await _context.Contacts.FindAsync(id);
            if (contact == null) return NotFound();
            return View(contact);
        }

        // POST: ContactController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            Contact contact = await _context.Contacts.FindAsync(id);
            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
