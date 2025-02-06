using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class ExportVM
    {
        public DateTime DateTime { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
        public int FromStoreId { get; set; }
    }
}
