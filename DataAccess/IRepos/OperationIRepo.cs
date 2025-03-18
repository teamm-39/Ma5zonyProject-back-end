using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Models;

namespace DataAccess.IRepos
{
    public interface OperationIRepo : IBaseRepo <Operation>
    {
        public int CreateOperation(int LkOperationType, string userId, int supplierOrCustomerId);
    }
}
