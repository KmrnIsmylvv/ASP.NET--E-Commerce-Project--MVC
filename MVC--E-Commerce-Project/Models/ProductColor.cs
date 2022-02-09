using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC__E_Commerce_Project.Models
{
    public class ProductColor
    {
        public int Id { get; set; }

        public int ColorId { get; set; }
        public Color Color { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
