using DataAccess.Data;
using DataAccess.IRepos;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Rpos
{
    public class OperationStoreProductRepo : BaseRepo<OperationStoreProduct>, OperationStoreProductIRepo
    {
        ApplicationDbContext _context;
        public OperationStoreProductRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public List<OperationStoreProduct> GetAllIds(int operationId)
        {
            var OPS = _context.OperationStoreProducts.Where(e => e.OperationId == operationId).ToList();
            return OPS;
        }
        public IQueryable<OperationStoreProduct> GetAllWithoutPagination( Expression<Func<OperationStoreProduct, object>>[]? includes = null,
     Expression<Func<OperationStoreProduct, bool>>? expression = null)
        {
            var model = _context.OperationStoreProducts;
            var query = model.AsQueryable();
            if (includes != null && includes.Length > 0)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            if (expression != null)
            {
                query = query.Where(expression);
            }
            return query;
        }
    }
}
