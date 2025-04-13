using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepos
{
    public interface CustomerSupplierLogIRepo:IBaseRepo<CustomerSupplierLog>
    {
        public void CreateOperationLog(CustomerSupplier? oldCustomerSupplier, CustomerSupplier? newCustomerSupplier, int operationType, string userId);

    }
}
