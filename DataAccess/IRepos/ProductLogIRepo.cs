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
    public interface ProductLogIRepo:IBaseRepo<ProductLog>
    {
        public void CreateOperationLog(Product? oldProduct, Product? newProduct, int operationType, string userId);
        public List<ProductLogVM> GetAllWithoutPagination(
Expression<Func<ProductLog, object>>[]? includes = null,
Expression<Func<ProductLog, bool>>? expression = null,
Dictionary<string, object>? filters = null);
    }
}
