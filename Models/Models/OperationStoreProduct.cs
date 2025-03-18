using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class OperationStoreProduct
    {
        public int OperationStoreProductId { get; set; }
        public int OperationId { get; set; }
        public int ProductId { get; set; }
        public int? FromStoreId { get; set; }
        public int? ToStoreId { get; set; }
        public int Quantity { get; set; }

        public Operation Operation { get; set; }
        public Product Product { get; set; }
        public Store FromStore { get; set; }
        public Store ToStore { get; set; }
    }
}
