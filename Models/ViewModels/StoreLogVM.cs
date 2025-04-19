using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class StoreLogVM
    {
        public int StoreLogId { get; set; }
        public string UserName { get; set; }
        public string OldName { get; set; }
        public string OldCountry { get; set; }
        public string OlgCity { get; set; }
        public string NewName { get; set; }
        public string NewCountry { get; set; }
        public string NewCity { get; set; }
        public int LookupOperationTypeId { get; set; }
        public string Message { get; set; }
    }
}
