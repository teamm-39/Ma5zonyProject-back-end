using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class StoreProducts
    {
        public int StoreId { get; set; }
        public Store Store { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
