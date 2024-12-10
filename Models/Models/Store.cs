using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class Store
    {
        public int StoreId { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        
        public ICollection<ApplicationUser> ApplicationUsers { get; set; }
    }
}
