using Models.Models;
using Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DataAccess.IRepos
{
    public interface StoreLogIRepo:IBaseRepo<StoreLog>
    {
        public void CreateOperationLog(Store? oldStore, Store? newStore, int operationType, string userId);
        public List<StoreLogVM> GetAllWithoutPagination(
Expression<Func<StoreLog, object>>[]? includes = null,
Expression<Func<StoreLog, bool>>? expression = null,
Dictionary<string, object>? filters = null);
    }
}
