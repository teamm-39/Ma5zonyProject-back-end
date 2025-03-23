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
    public interface OperationStoreProductIRepo : IBaseRepo<OperationStoreProduct>
    {
        public List<OperationStoreProduct> GetAllIds(int operationId);
        public IQueryable<OperationStoreProduct> GetAllWithoutPagination( Expression<Func<OperationStoreProduct, object>>[]? includes = null,
Expression<Func<OperationStoreProduct, bool>>? expression = null);
    }
}
