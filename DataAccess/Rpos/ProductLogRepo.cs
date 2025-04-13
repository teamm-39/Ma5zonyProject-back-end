using DataAccess.Data;
using DataAccess.IRepos;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DataAccess.Rpos
{
    public class ProductLogRepo : BaseRepo<ProductLog>, ProductLogIRepo
    {
        ApplicationDbContext _context;

        public ProductLogRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public void CreateOperationLog(Product? oldProduct, Product? newProduct, int operationType, string userId)
        {
            var log = new ProductLog();
            if (operationType == StaticData.AddOperationType)
            {
                log = new ProductLog { ApplicationUserId = userId, LookupOperationTypeId = operationType, OldMinLimit = 0, OldName = "-", OldPurchasePrice = 0, OldSellingPrice = 0, NewMinLimit = newProduct.MinLimit, NewName = newProduct.Name, NewPurchasePrice = newProduct.PurchasePrice, NewSellingPrice = newProduct.SellingPrice,Message="تم اضافة المنتج" };
            }
            else if (operationType == StaticData.EditOperationType)
            {
                log = new ProductLog { ApplicationUserId = userId, LookupOperationTypeId = operationType, OldMinLimit = oldProduct.MinLimit, OldName = oldProduct.Name, OldPurchasePrice = oldProduct.PurchasePrice, OldSellingPrice = oldProduct.SellingPrice, NewMinLimit = newProduct.MinLimit, NewName = newProduct.Name, NewPurchasePrice = newProduct.PurchasePrice, NewSellingPrice = newProduct.SellingPrice,Message="تم تعديل المنتج" };
            }
            else
            {
                log = new ProductLog { ApplicationUserId = userId, LookupOperationTypeId = operationType, OldMinLimit = oldProduct.MinLimit, OldName = oldProduct.Name, OldPurchasePrice = oldProduct.PurchasePrice, OldSellingPrice = oldProduct.SellingPrice, NewMinLimit = 0, NewName = "-", NewPurchasePrice = 0, NewSellingPrice = 0 , Message="تم حذف المنتج" };
            }
            log.DateTime=DateTime.Now;
            _context.Add(log);
            _context.SaveChanges();
        }
    }
}