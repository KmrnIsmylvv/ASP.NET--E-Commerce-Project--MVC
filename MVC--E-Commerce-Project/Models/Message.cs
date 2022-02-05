using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC__E_Commerce_Project.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }
    }
}
