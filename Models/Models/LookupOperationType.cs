using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class LookupOperationType
    {
        public int LookupOperationTypeId { get; set; }
        public string Name {  get; set; }

        public ICollection<Operation> Operations { get; set; }
    }
}
