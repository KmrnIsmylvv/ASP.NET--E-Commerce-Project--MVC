using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVC__E_Commerce_Project.Models
{
    public class Brand
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Can't be empty")]
        public string Name { get; set; }

        public List<BrandCategory> BrandCategories { get; set; }
    }
}
