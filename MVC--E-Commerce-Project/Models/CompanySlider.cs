using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MVC__E_Commerce_Project.Models
{
    public class CompanySlider
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }

        [Required]
        [NotMapped]
        public IFormFile Image { get; set; }
    }
}
