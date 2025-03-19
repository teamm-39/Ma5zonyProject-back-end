using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class ImportOperationCreateVM
    {
        public int SupplierId { get; set; }
        public List<StoreProductForCreatImportOperationVM> SP { get; set; }
    }
}
