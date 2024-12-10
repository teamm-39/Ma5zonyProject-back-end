using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class Export
    {
        public int ExportId { get; set; }
        public DateTime DateTime { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
        public int FromStoreId { get; set; }

        public string ApplicationUserId  { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
