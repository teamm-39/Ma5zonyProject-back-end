using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class ProductEditVM
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public double SellingPrice { get; set; }
        public double PurchasePrice { get; set; }
        public int MinLimit { get; set; }
    }
}
