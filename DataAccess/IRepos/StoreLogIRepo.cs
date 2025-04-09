using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepos
{
    public interface StoreLogIRepo:IBaseRepo<StoreLog>
    {
        public void CreateOperationLog(Store? oldStore, Store? newStore, int operationType, string userId);

    }
}
