using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class StoreLog
    {
        public int StoreLogId { get; set; }
        public string ApplicationUserId { get; set; }
        public string OldName { get; set; }
        public string OldCountry { get; set; }
        public string OldCity { get; set; }
        public string NewName { get; set; }
        public string NewCountry { get; set; }
        public string NewCity { get; set; }
        public string Message   { get; set; }
        public DateTime DateTime { get; set; }
        public int LookupOperationTypeId { get; set; }

        public LookupOperationType LookupOperationType { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
