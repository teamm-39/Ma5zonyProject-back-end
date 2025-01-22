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
    public class CustomerRepo : BaseRepo<Customer>, CustomerIRepo
    {
        ApplicationDbContext _context;
        public CustomerRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
