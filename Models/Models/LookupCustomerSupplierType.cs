using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class LookupCustomerSupplierType
    {
        public int LookupCustomerSupplierTypeId { get; set; }
        public string Name { get; set; }
        public ICollection<CustomerSupplier> CustomerSuppliers { get; set; }
    }
}
