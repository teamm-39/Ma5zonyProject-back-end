﻿using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepos
{
    public interface OperationStoreProductIRepo : IBaseRepo<OperationStoreProduct>
    {
        public List<OperationStoreProduct> GetAllIds(int operationId);
    }
}
