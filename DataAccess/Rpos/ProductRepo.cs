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
    public class ProductRepo : BaseRepo<Product>, ProductIRepo
    {
        ApplicationDbContext _context;
        public ProductRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public List<ProductForOperation> GetProductsForOperations()
        {
            var products = _context.Products.Where(e=>e.IsDeleted==false).Select(e => new ProductForOperation { ProductId = e.ProductId, ProductName = e.Name,Price=e.PurchasePrice }).ToList();
            return products;
        }
    }
}
