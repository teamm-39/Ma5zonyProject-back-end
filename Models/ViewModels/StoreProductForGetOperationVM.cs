using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class StoreProductForGetOperationVM
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public int? ToStoreId { get; set; }
        public int? FromStoreId { get; set; }
        public string StoreName { get; set; }
    }
}
