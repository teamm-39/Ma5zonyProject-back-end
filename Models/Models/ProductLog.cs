using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class ProductLog
    {
        public int ProductLogId { get; set; }
        public string OldName { get; set; }
        public double OldSellingPrice { get; set; }
        public double OldPurchasePrice { get; set; }
        public int OldMinLimit { get; set; }
        public string NewName { get; set; }
        public double NewSellingPrice { get; set; }
        public double NewPurchasePrice { get; set; }
        public int NewMinLimit { get; set; }
        public DateTime DateTime { get; set; }
        public int LookupOperationTypeId { get; set; }
        public string ApplicationUserId { get; set; }
        public string Message   { get; set; }

        public LookupOperationType LookupOperationType { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
