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
    public class StoreRepo : BaseRepo<Store>, StoreIRepo
    {
        ApplicationDbContext _context;
        public StoreRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
