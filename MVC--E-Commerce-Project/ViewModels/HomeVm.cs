using MVC__E_Commerce_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC__E_Commerce_Project.ViewModels
{
    public class HomeVm
    {
        public List<CompanySlider> companySliders { get; set; }
        public List<Service> services { get; set; }
        public List<Category> categories { get; set; }
    }
}
