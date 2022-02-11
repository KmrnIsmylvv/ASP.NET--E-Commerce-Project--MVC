using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC__E_Commerce_Project.Models
{
    public class Comments
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }
    }
}
