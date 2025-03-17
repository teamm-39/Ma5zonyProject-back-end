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
        public double SellingPrice { get; set; }
        public double PurchasePrice { get; set; }
        public int Quantity { get; set; }
        public int MinLimit { get; set; }
        public bool IsDeleted { get; set; }
    }
}
