using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVC__E_Commerce_Project.Models
{
    public class AppUser : IdentityUser
    {
        [Required, StringLength(50)]
        public string FullName { get; set; }
        public bool IsSubscribe { get; set; }
        public string Photo { get; set; }

        public List<Message> Messages { get; set; }
        public List<Comments> Comments { get; set; }

    }
}
