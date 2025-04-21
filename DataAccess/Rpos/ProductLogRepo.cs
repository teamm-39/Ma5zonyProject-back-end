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
        public List<ProductLogVM> GetAllWithoutPagination(
Expression<Func<ProductLog, object>>[]? includes = null,
Expression<Func<ProductLog, bool>>? expression = null,
Dictionary<string, object>? filters = null)
        {
            IQueryable<ProductLog> query = _context.ProductLogs.AsQueryable();

            // إضافة Include للعلاقات
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
            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    var properties = filter.Key.Split('.'); // تقسيم الـ Key لمعرفة إن كان يحتوي على علاقة
                    var parameter = Expression.Parameter(typeof(ProductLog), "x");
                    Expression propertyAccess = parameter;

                    foreach (var prop in properties)
                    {
                        var property = propertyAccess.Type.GetProperty(prop);
                        if (property == null) break;
                        propertyAccess = Expression.Property(propertyAccess, property);
                    }

                    if (propertyAccess.Type == typeof(string))
                    {
                        var value = Expression.Constant(filter.Value.ToString()?.ToLower());
                        var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

                        var propertyLower = Expression.Call(propertyAccess, toLowerMethod);
                        var containsExpression = Expression.Call(propertyLower, containsMethod, value);

                        var lambda = Expression.Lambda<Func<ProductLog, bool>>(containsExpression, parameter);
                        query = query.Where(lambda);
                    }
                    else if (propertyAccess.Type == typeof(DateTime) || propertyAccess.Type == typeof(DateTime?))
                    {
                        if (DateTime.TryParse(filter.Value.ToString(), out DateTime dateValue))
                        {
                            var value = Expression.Constant(dateValue, typeof(DateTime));
                            var greaterThanOrEqualExpression = Expression.GreaterThanOrEqual(propertyAccess, value);
                            var lambda = Expression.Lambda<Func<ProductLog, bool>>(greaterThanOrEqualExpression, parameter);
                            query = query.Where(lambda);
                        }
                    }
                    else if (propertyAccess.Type == typeof(int) || propertyAccess.Type == typeof(int?))
                    {
                        if (int.TryParse(filter.Value.ToString(), out int intValue))
                        {
                            var value = Expression.Constant(intValue);
                            var equalExpression = Expression.Equal(propertyAccess, value);
                            var lambda = Expression.Lambda<Func<ProductLog, bool>>(equalExpression, parameter);
                            query = query.Where(lambda);
                        }
                    }
                    else if (propertyAccess.Type == typeof(double) || propertyAccess.Type == typeof(double?))
                    {
                        if (double.TryParse(filter.Value.ToString(), out double doubleValue))
                        {
                            var value = Expression.Constant(doubleValue);
                            var equalExpression = Expression.Equal(propertyAccess, value);
                            var lambda = Expression.Lambda<Func<ProductLog, bool>>(equalExpression, parameter);
                            query = query.Where(lambda);
                        }
                    }
                }
            }
            var data = query.Select(e => new ProductLogVM
            {
                LookupOperationTypeId = e.LookupOperationTypeId,
                Message = e.Message,
                DateTime = e.DateTime,
                NewMinLimit = e.NewMinLimit,
                NewName = e.NewName,
                NewPurchasePrice = e.NewPurchasePrice,
                NewSellingPrice = e.NewSellingPrice,
                OldMinLimit = e.OldMinLimit,
                OldName = e.OldName,
                OldPurchasePrice = e.OldPurchasePrice,
                OldSellingPrice = e.OldSellingPrice,
                ProductLogId = e.ProductLogId,
                UserName=e.ApplicationUser.Name
            }).ToList();
            return data;
        }
    }
}