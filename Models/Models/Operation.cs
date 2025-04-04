﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class Operation
    {
        public int OperationId { get; set; }
        public DateTime DateTime { get; set; }
        public int LookupOperationTypeId { get; set; }
        public string ApplicationUserId { get; set; }
        public int CustomerSupplierId { get; set; }
        public double? TotalPrice { get; set; }
        public bool IsDeleted { get; set; }

        public CustomerSupplier CustomerSupplier { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public LookupOperationType LookupOperationType { get; set; }
    }
}
