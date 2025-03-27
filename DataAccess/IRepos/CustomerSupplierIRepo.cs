using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Models;
using Models.ViewModels;

namespace DataAccess.IRepos
{
    public interface CustomerSupplierIRepo:IBaseRepo<CustomerSupplier>
    {
        public List<SupplierOrCustomerForOperation> GetSuppliersOrCustomersForOperation(int lkType);
    }
}
