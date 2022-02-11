using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MVC__E_Commerce_Project.Models
{
    public class HomeProductSlider
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }
        [Required]
        public string ImageUrl { get; set; }

        [NotMapped]
        [Required]
        public IFormFile Photo { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
