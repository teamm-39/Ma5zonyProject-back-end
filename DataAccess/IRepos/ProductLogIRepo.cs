using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepos
{
    public interface ProductLogIRepo:IBaseRepo<ProductLog>
    {
        public void CreateOperationLog(Product? oldProduct, Product? newProduct, int operationType, string userId);

    }
}
