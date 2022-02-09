using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC__E_Commerce_Project.Models
{
    public class Color
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<ProductColor> ProductColors { get; set; }
    }
}
