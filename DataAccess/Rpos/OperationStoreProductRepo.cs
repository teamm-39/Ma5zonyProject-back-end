using DataAccess.Data;
using DataAccess.IRepos;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Rpos
{
    public class OperationStoreProductRepo : BaseRepo<OperationStoreProduct> , OperationStoreProductIRepo
    {
        ApplicationDbContext _context;
        public OperationStoreProductRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
