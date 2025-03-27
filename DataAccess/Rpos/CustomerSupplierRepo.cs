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
    public class CustomerSupplierRepo : BaseRepo<CustomerSupplier>, CustomerSupplierIRepo
    {
        ApplicationDbContext _context;
        public CustomerSupplierRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public List<SupplierOrCustomerForOperation> GetSuppliersOrCustomersForOperation(int lkType)
        {
            var suppliers= _context.CustomersSuppliers.Where(e => e.IsDeleted == false&&e.LookupCustomerSupplierTypeId==lkType).Select(s=>new SupplierOrCustomerForOperation { Id=s.CustomerSupplierId,Name=s.Name }).ToList();
            return suppliers;
        }
    }
}
