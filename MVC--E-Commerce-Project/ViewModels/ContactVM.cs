using MVC__E_Commerce_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC__E_Commerce_Project.ViewModels
{
    public class ContactVM
    {
        public AppUser User { get; set; }
        public Contact Contact { get; set; }
        public Message Message { get; set; }

    }
}
