﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class CustomerSupplier
    {
        public int CustomerSupplierId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public int NumOfDeal { get; set; }
        public bool IsReliable { get; set; }
        public bool IsDeleted { get; set; }
        public int LookupCustomerSupplierTypeId { get; set; }
        public LookupCustomerSupplierType LookupCustomerSupplierType { get; set; }
    }
}
