using Microsoft.EntityFrameworkCore;
using MVC__E_Commerce_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC__E_Commerce_Project.DAL
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {

        }
        public DbSet<About> Abouts { get; set; }
        public DbSet<CompanySlider> CompanySliders { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Subscribe> Subscribes { get; set; }
    }
}
