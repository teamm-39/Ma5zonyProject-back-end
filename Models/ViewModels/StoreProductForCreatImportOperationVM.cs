using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class StoreProductForCreatImportOperationVM
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int ToStoreId { get; set; }
    }
}
