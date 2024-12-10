using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }

        public ICollection<StoreProducts> StoreProducts { get; set; }
        public ICollection<ProductExports> ProductExports { get; set; }
        public ICollection<ProductImports> ProductImports { get; set; }
    }
}
