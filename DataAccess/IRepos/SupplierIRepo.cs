using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Models;
using Models.ViewModels;

namespace DataAccess.IRepos
{
    public interface SupplierIRepo:IBaseRepo<Supplier>
    {
        public List<SupplierForOperation> GetSuppliersForOperation();
    }
}
