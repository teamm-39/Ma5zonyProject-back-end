using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class ProductImports
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int ImportId { get; set; }
        public Import Import { get; set; }
    }
}
