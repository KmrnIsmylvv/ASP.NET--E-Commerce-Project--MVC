using MVC__E_Commerce_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC__E_Commerce_Project.ViewModels
{
    public class UserRoleVM
    {
        public AppUser AppUser;

        public IList<string> Roles;
    }
}
