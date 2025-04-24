using Models.Models;
using Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepos
{
    public interface CustomerSupplierLogIRepo:IBaseRepo<CustomerSupplierLog>
    {
        public void CreateOperationLog(CustomerSupplier? oldCustomerSupplier, CustomerSupplier? newCustomerSupplier, int operationType, string userId);
        public List<CustomerSupplierLogVM> GetAllWithoutPagination(
Expression<Func<CustomerSupplierLog, object>>[]? includes = null,
Expression<Func<CustomerSupplierLog, bool>>? expression = null,
Dictionary<string, object>? filters = null);
    }
}
