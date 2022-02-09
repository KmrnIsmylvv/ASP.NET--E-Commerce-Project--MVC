using Microsoft.AspNetCore.Identity;
using MVC__E_Commerce_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC__E_Commerce_Project.ViewModels
{
    public class UpdateRoleVM
    {
        public IList<IdentityRole> Roles { get; set; }

        public IList<string> UserRoles { get; set; }

        public string UserId { get; set; }

        public AppUser User { get; set; }
    }
}
