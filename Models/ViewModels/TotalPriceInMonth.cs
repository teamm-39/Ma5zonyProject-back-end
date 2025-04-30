using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class TotalPriceInMonth
    {
        public string MonthName { get; set; }
        public int MonthNumber { get; set; }
        public double? TotalPrice { get; set; }
    }
}
