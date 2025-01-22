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
    public class ProductImportRepo : BaseRepo<ProductImports>, ProductImportIRepo
    {
        ApplicationDbContext _context;
        public ProductImportRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }

}