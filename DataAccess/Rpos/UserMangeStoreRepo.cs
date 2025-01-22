using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Data;
using DataAccess.IRepos;
using Models.Models;

namespace DataAccess.Rpos
{
    public class UserMangeStoreRepo : BaseRepo<UserMangeStore>, UserMangerStoreIRepo
    {
        ApplicationDbContext _context;
        public UserMangeStoreRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}