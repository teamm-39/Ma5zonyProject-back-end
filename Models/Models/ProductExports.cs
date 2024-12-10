using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class ProductExports
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int ExportId { get; set; }
        public Export Export { get; set; }
    }
}
