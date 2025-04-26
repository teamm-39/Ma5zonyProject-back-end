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
    public class ApplicationUserLogRepo : BaseRepo<ApplicationUserLog>, ApplicationUserLogIRepo
    {
        ApplicationDbContext _context;
        public ApplicationUserLogRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public void CreateOperationLog(ApplicationUser? oldUser, ApplicationUser? newUser, int operationType, string userId, string roleName)
        {
            var log = new ApplicationUserLog();
            if (operationType == StaticData.AddOperationType)
            {
                log = new ApplicationUserLog
                {
                    ApplicationUserId = userId,
                    RoleName = roleName,
                    OldAddress = "-",
                    OldAge = 0,
                    OldEmail = "-",
                    OldImgUrl = "-",
                    OldName = "-",
                    OldPhoneNumber = "-",
                    OldUserName = "-",
                    NewAddress = newUser.Address,
                    NewAge = newUser.Age,
                    NewEmail = newUser.Email,
                    NewImgUrl = newUser.ImgUrl,
                    NewName = newUser.Name,
                    NewPhoneNumber = newUser.PhoneNumber,
                    NewUserName = newUser.UserName,
                    Message = "تم اضافة المستخدم"
                };
            }
            else if (operationType == StaticData.EditOperationType)
            {
                log = new ApplicationUserLog
                {
                    ApplicationUserId = userId,
                    RoleName = roleName,
                    OldAddress = oldUser.Address,
                    OldAge = oldUser.Age,
                    OldEmail = oldUser.Email,
                    OldImgUrl = oldUser.ImgUrl,
                    OldName = oldUser.Name,
                    OldPhoneNumber = oldUser.PhoneNumber,
                    OldUserName = oldUser.UserName,
                    NewAddress = newUser.Address,
                    NewAge = newUser.Age,
                    NewEmail = newUser.Email,
                    NewImgUrl = newUser.ImgUrl,
                    NewName = newUser.Name,
                    NewPhoneNumber = newUser.PhoneNumber,
                    NewUserName = newUser.UserName,
                    Message = "تم تعديل المستخدم"
                };
            }
            else
            {
                log = new ApplicationUserLog
                {
                    ApplicationUserId = userId,
                    RoleName = roleName,
                    OldAddress = oldUser.Address,
                    OldAge = oldUser.Age,
                    OldEmail = oldUser.Email,
                    OldImgUrl = oldUser.ImgUrl,
                    OldName = oldUser.Name,
                    OldPhoneNumber = oldUser.PhoneNumber,
                    OldUserName = oldUser.UserName,
                    NewAddress = "-",
                    NewAge = 0,
                    NewEmail = "-",
                    NewImgUrl = "-",
                    NewName = "-",
                    NewPhoneNumber = "-",
                    NewUserName = "-",
                    Message = "تم حذف المستخدم"
                };
            }
            log.LookupOperationTypeId = operationType;
            log.DateTime = DateTime.Now;
            _context.ApplicationUserLogs.Add(log);
            _context.SaveChanges();
        }
        public List<ApplicationUserLogVM> GetAllWithoutPagination(
Expression<Func<ApplicationUserLog, object>>[]? includes = null,
Expression<Func<ApplicationUserLog, bool>>? expression = null,
Dictionary<string, object>? filters = null)
        {
            IQueryable<ApplicationUserLog> query = _context.ApplicationUserLogs.AsQueryable();
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
                    var parameter = Expression.Parameter(typeof(ApplicationUserLog), "x");
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

                        var lambda = Expression.Lambda<Func<ApplicationUserLog, bool>>(containsExpression, parameter);
                        query = query.Where(lambda);
                    }
                    else if (propertyAccess.Type == typeof(DateTime) || propertyAccess.Type == typeof(DateTime?))
                    {
                        if (DateTime.TryParse(filter.Value.ToString(), out DateTime dateValue))
                        {
                            var value = Expression.Constant(dateValue, typeof(DateTime));
                            var greaterThanOrEqualExpression = Expression.GreaterThanOrEqual(propertyAccess, value);
                            var lambda = Expression.Lambda<Func<ApplicationUserLog, bool>>(greaterThanOrEqualExpression, parameter);
                            query = query.Where(lambda);
                        }
                    }
                    else if (propertyAccess.Type == typeof(int) || propertyAccess.Type == typeof(int?))
                    {
                        if (int.TryParse(filter.Value.ToString(), out int intValue))
                        {
                            var value = Expression.Constant(intValue);
                            var equalExpression = Expression.Equal(propertyAccess, value);
                            var lambda = Expression.Lambda<Func<ApplicationUserLog, bool>>(equalExpression, parameter);
                            query = query.Where(lambda);
                        }
                    }
                    else if (propertyAccess.Type == typeof(double) || propertyAccess.Type == typeof(double?))
                    {
                        if (double.TryParse(filter.Value.ToString(), out double doubleValue))
                        {
                            var value = Expression.Constant(doubleValue);
                            var equalExpression = Expression.Equal(propertyAccess, value);
                            var lambda = Expression.Lambda<Func<ApplicationUserLog, bool>>(equalExpression, parameter);
                            query = query.Where(lambda);
                        }
                    }
                }
            }
            var data = query.Select(e => new ApplicationUserLogVM
            {
                LookupOperationTypeId = e.LookupOperationTypeId,
                UserName = e.ApplicationUser.Name,
                ApplicationUserLogId = e.ApplicationUserLogId,
                DateTime = e.DateTime,
                Message = e.Message,
                NewAddress = e.NewAddress,
                NewAge = e.NewAge,
                NewEmail = e.NewEmail,
                NewImgUrl = e.NewImgUrl,
                NewName = e.NewName,
                NewPhoneNumber = e.NewPhoneNumber,
                NewUserName = e.NewUserName,
                OldAddress = e.OldAddress,
                OldAge = e.OldAge,
                OldEmail = e.OldEmail,
                OldImgUrl = e.OldImgUrl,
                OldName = e.OldName,
                OldPhoneNumber = e.OldPhoneNumber,
                OldUserName = e.OldUserName,

            }).ToList();
            return data;
        }
    }
}
