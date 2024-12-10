using DataAccess.Data;
using DataAccess.IRepos;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Rpos
{
    public class ImportRepo:BaseRepo<Import> , ImportIRepo
    {
        ApplicationDbContext _context;
        public ImportRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
