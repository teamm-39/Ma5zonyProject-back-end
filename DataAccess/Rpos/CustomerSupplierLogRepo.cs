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
    public class CustomerSupplierLogRepo : BaseRepo<CustomerSupplierLog>, CustomerSupplierLogIRepo
    {
        ApplicationDbContext _context;
        public CustomerSupplierLogRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public void CreateOperationLog(CustomerSupplier? oldCustomerSupplier, CustomerSupplier? newCustomerSupplier, int operationType, string userId)
        {
            var log = new CustomerSupplierLog();
            if (operationType == StaticData.AddOperationType)
            {
                log = new CustomerSupplierLog
                {
                    ApplicationUserId = userId,
                    LookupOperationTypeId = operationType,
                    LookupCustomerSupplierTypeId = newCustomerSupplier.LookupCustomerSupplierTypeId,
                    Message = newCustomerSupplier.LookupCustomerSupplierTypeId == 1 ? "تم اضافة المورد" : "تم اضافة العميل",
                    OldAddress = "-",
                    OldAge = 0,
                    OldEmail = "-",
                    OldIsReliable = false,
                    OldName = "-",
                    OldPhoneNumber = "-",
                    NewAddress = newCustomerSupplier.Address,
                    NewAge = newCustomerSupplier.Age,
                    NewEmail = newCustomerSupplier.Email,
                    NewIsReliable = newCustomerSupplier.IsReliable,
                    NewName = newCustomerSupplier.Name,
                    NewPhoneNumber = newCustomerSupplier.PhoneNumber
                };
            }
            else if (operationType == StaticData.EditOperationType)
            {
                log = new CustomerSupplierLog
                {
                    ApplicationUserId = userId,
                    LookupOperationTypeId = operationType,
                    LookupCustomerSupplierTypeId = oldCustomerSupplier.LookupCustomerSupplierTypeId,
                    Message = oldCustomerSupplier.LookupCustomerSupplierTypeId == 1 ? "تم تعديل المورد" : "تم تعديل العميل",
                    OldAddress = oldCustomerSupplier.Address,
                    OldAge = oldCustomerSupplier.Age,
                    OldEmail = oldCustomerSupplier.Email,
                    OldIsReliable = oldCustomerSupplier.IsReliable,
                    OldName = oldCustomerSupplier.Name,
                    OldPhoneNumber = oldCustomerSupplier.PhoneNumber,
                    NewAddress = newCustomerSupplier.Address,
                    NewAge = newCustomerSupplier.Age,
                    NewEmail = newCustomerSupplier.Email,
                    NewIsReliable = newCustomerSupplier.IsReliable,
                    NewName = newCustomerSupplier.Name,
                    NewPhoneNumber = newCustomerSupplier.PhoneNumber
                };
            }
            else
            {
                log = new CustomerSupplierLog
                {
                    ApplicationUserId = userId,
                    LookupOperationTypeId = operationType,
                    LookupCustomerSupplierTypeId = oldCustomerSupplier.LookupCustomerSupplierTypeId,
                    Message = oldCustomerSupplier.LookupCustomerSupplierTypeId == 1 ? "تم حذف المورد" : "تم حذف العميل",
                    NewAddress = "-",
                    NewAge = 0,
                    NewEmail = "-",
                    NewIsReliable = false,
                    NewName = "-",
                    NewPhoneNumber = "-",
                    OldAddress = oldCustomerSupplier.Address,
                    OldAge = oldCustomerSupplier.Age,
                    OldEmail = oldCustomerSupplier.Email,
                    OldIsReliable = oldCustomerSupplier.IsReliable,
                    OldName = oldCustomerSupplier.Name,
                    OldPhoneNumber = oldCustomerSupplier.PhoneNumber
                };
            }
            log.DateTime = DateTime.Now;
            _context.CustomerSupplierLogs.Add(log);
            _context.SaveChanges();
        }
        public List<CustomerSupplierLogVM> GetAllWithoutPagination(
Expression<Func<CustomerSupplierLog, object>>[]? includes = null,
Expression<Func<CustomerSupplierLog, bool>>? expression = null,
Dictionary<string, object>? filters = null)
        {
            IQueryable<CustomerSupplierLog> query = _context.CustomerSupplierLogs.AsQueryable();

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
                    var parameter = Expression.Parameter(typeof(CustomerSupplierLog), "x");
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

                        var lambda = Expression.Lambda<Func<CustomerSupplierLog, bool>>(containsExpression, parameter);
                        query = query.Where(lambda);
                    }
                    else if (propertyAccess.Type == typeof(DateTime) || propertyAccess.Type == typeof(DateTime?))
                    {
                        if (DateTime.TryParse(filter.Value.ToString(), out DateTime dateValue))
                        {
                            var value = Expression.Constant(dateValue, typeof(DateTime));
                            var greaterThanOrEqualExpression = Expression.GreaterThanOrEqual(propertyAccess, value);
                            var lambda = Expression.Lambda<Func<CustomerSupplierLog, bool>>(greaterThanOrEqualExpression, parameter);
                            query = query.Where(lambda);
                        }
                    }
                    else if (propertyAccess.Type == typeof(int) || propertyAccess.Type == typeof(int?))
                    {
                        if (int.TryParse(filter.Value.ToString(), out int intValue))
                        {
                            var value = Expression.Constant(intValue);
                            var equalExpression = Expression.Equal(propertyAccess, value);
                            var lambda = Expression.Lambda<Func<CustomerSupplierLog, bool>>(equalExpression, parameter);
                            query = query.Where(lambda);
                        }
                    }
                    else if (propertyAccess.Type == typeof(double) || propertyAccess.Type == typeof(double?))
                    {
                        if (double.TryParse(filter.Value.ToString(), out double doubleValue))
                        {
                            var value = Expression.Constant(doubleValue);
                            var equalExpression = Expression.Equal(propertyAccess, value);
                            var lambda = Expression.Lambda<Func<CustomerSupplierLog, bool>>(equalExpression, parameter);
                            query = query.Where(lambda);
                        }
                    }
                }
            }
            var data = query.Select(e => new CustomerSupplierLogVM
            {
                CustomerSupplierLogId = e.CustomerSupplierLogId,
                DateTime = e.DateTime,
                LookupOperationTypeId = e.LookupOperationTypeId,
                Message = e.Message,
                NewAddress = e.NewAddress,
                NewAge = e.NewAge,
                NewEmail = e.NewEmail,
                NewIsReliable = e.NewIsReliable,
                NewPhoneNumber = e.NewPhoneNumber,
                NewName = e.NewName,
                OldAddress = e.OldAddress,
                OldAge = e.OldAge,
                OldEmail = e.OldEmail,
                OldIsReliable = e.OldIsReliable,
                OldName = e.OldName,
                OldPhoneNumber = e.OldPhoneNumber,
                UserName = e.ApplicationUser.Name
            }).ToList();
            return data;
        }
    }
}
