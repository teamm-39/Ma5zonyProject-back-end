using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string? ImgUrl { get; set; }
        public string? Address { get; set; }
        public ICollection<UserMangeProduct> UserMangeProducts { get; set; }
        public ICollection<Export> Exports { get; set; }
        public ICollection<Import> Imports { get; set; }
    }
}
