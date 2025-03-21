using DataAccess.Data;
using DataAccess.IRepos;
using Models.Models;
using Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataAccess.Rpos
{
    public class StoreRepo : BaseRepo<Store>, StoreIRepo
    {
        ApplicationDbContext _context;
        public StoreRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public List<StoreForOperation> GetStoresForOperations()
        {
            var stores=_context.Stores.Where(e => e.IsDeleted == false).Select(e=>new StoreForOperation { StoreId=e.StoreId,StoreName=e.Name}).ToList();
            return stores;
        }
    }
}
