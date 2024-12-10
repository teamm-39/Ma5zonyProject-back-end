using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class Import
    {
        public int ImportId { get; set; }
        public DateTime DateTime { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
        public int ToStoreId { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }
    }
}
