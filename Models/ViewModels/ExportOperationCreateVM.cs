﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class ExportOperationCreateVM
    {
        public int CustomerId { get; set; }
        public List<StoreProductForCreateExportOperationVM> SP {  get; set; }
    }
}
