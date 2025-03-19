using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class ImportsVM
    {
        public int OperationId { get; set; }
        public DateTime DateTime { get; set; }
        public string UserName { get; set; }
        public double TotalPrice { get; set; }
        public string SupplierName { get; set; }
    }
}
