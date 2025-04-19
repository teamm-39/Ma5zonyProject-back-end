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
    public class StoreLogRepo : BaseRepo<StoreLog>, StoreLogIRepo
    {
        ApplicationDbContext _context;
        public StoreLogRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public void CreateOperationLog(Store? oldStore, Store? newStore, int operationType, string userId)
        {
            var storeLog = new StoreLog();
            if (operationType == StaticData.AddOperationType)
            {
                storeLog = new StoreLog { ApplicationUserId = userId, OldName = "-", OldCountry = "-", OldCity = "-", NewName = newStore.Name, NewCountry = newStore.Country, NewCity = newStore.City, Message = "تم اضافة المخزن" };
            }
            else if (operationType == StaticData.EditOperationType)
            {
                storeLog = new StoreLog { ApplicationUserId = userId, OldName = oldStore.Name, OldCountry = oldStore.Country, OldCity = oldStore.City, NewName = newStore.Name, NewCountry = newStore.Country, NewCity = newStore.City, Message = "تم تعديل المخزن" };

            }
            else
            {
                storeLog = new StoreLog { ApplicationUserId = userId, OldName = oldStore.Name, OldCountry = oldStore.Country, OldCity = oldStore.City, NewName = "-", NewCountry = "-", NewCity = "-", Message = "تم حذف المخزن" };
            }
            storeLog.LookupOperationTypeId = operationType;
            storeLog.DateTime = DateTime.Now;
            _context.StoreLogs.Add(storeLog);
            _context.SaveChanges();
        }
        public List<StoreLogVM> GetAllWithoutPagination(
Expression<Func<StoreLog, object>>[]? includes = null,
Expression<Func<StoreLog, bool>>? expression = null,
Dictionary<string, object>? filters = null)
        {
            IQueryable<StoreLog> query = _context.StoreLogs.AsQueryable();

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
                    var parameter = Expression.Parameter(typeof(StoreLog), "x");
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

                        var lambda = Expression.Lambda<Func<StoreLog, bool>>(containsExpression, parameter);
                        query = query.Where(lambda);
                    }
                    else if (propertyAccess.Type == typeof(DateTime) || propertyAccess.Type == typeof(DateTime?))
                    {
                        if (DateTime.TryParse(filter.Value.ToString(), out DateTime dateValue))
                        {
                            var value = Expression.Constant(dateValue, typeof(DateTime));
                            var greaterThanOrEqualExpression = Expression.GreaterThanOrEqual(propertyAccess, value);
                            var lambda = Expression.Lambda<Func<StoreLog, bool>>(greaterThanOrEqualExpression, parameter);
                            query = query.Where(lambda);
                        }
                    }
                    else if (propertyAccess.Type == typeof(int) || propertyAccess.Type == typeof(int?))
                    {
                        if (int.TryParse(filter.Value.ToString(), out int intValue))
                        {
                            var value = Expression.Constant(intValue);
                            var equalExpression = Expression.Equal(propertyAccess, value);
                            var lambda = Expression.Lambda<Func<StoreLog, bool>>(equalExpression, parameter);
                            query = query.Where(lambda);
                        }
                    }
                }
            }
            var data = query.Select(e => new StoreLogVM {
            LookupOperationTypeId=e.LookupOperationTypeId,
            Message=e.Message,
            NewCity=e.NewCity,
            NewCountry=e.NewCountry,
            NewName=e.NewName,OldCountry=e.OldName,OldName=e.OldName,OlgCity=e.OldCity,StoreLogId=e.StoreLogId,UserName=e.ApplicationUser.Name
            }).ToList();
            return data;
        }
    }
}
