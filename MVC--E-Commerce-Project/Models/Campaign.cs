using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC__E_Commerce_Project.Models
{
    public class Campaign
    {
        public int Id { get; set; }
        public int Discount { get; set; }

        public List<Product> Products { get; set; }
    }
}
