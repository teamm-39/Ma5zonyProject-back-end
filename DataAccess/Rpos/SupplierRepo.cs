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
    public class SupplierRepo : BaseRepo<Supplier>, SupplierIRepo
    {
        ApplicationDbContext _context;
        public SupplierRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public List<SupplierForOperation> GetSuppliersForOperation()
        {
            var suppliers= _context.Suppliers.Where(e => e.IsDeleted == false).Select(s=>new SupplierForOperation { SupplierId=s.SupplierId,SupplierName=s.Name }).ToList();
            return suppliers;
        }
    }
}
