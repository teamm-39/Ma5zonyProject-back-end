using DataAccess.Data;
using DataAccess.IRepos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Data;
using Utility;
namespace DataAccess.Rpos
{
    public class BaseRepo<T> : IBaseRepo<T> where T : class
    {
        ApplicationDbContext _context;
        DbSet<T> _model;
        public BaseRepo(ApplicationDbContext context)
        {
            _context = context;
            _model = _context.Set<T>();
        }
        public Result<IEnumerable<T>> GetAll(
     Expression<Func<T, object>>[]? includes = null,
     Expression<Func<T, bool>>? expression = null,
     Func<IQueryable<T>, IQueryable<T>>? additionalIncludes = null,
     int pageSize = 10,
     int pageNumber = 1,
     Dictionary<string, object>? filters = null)
        {
            var res = new Result<IEnumerable<T>>();
            var query = _model.AsQueryable();

            // إضافة Include للعلاقات
            if (includes != null && includes.Length > 0)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            // تطبيق الفلترة الأساسية
            if (expression != null)
            {
                query = query.Where(expression);
            }

            // تطبيق الفلترة الديناميكية
            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    var properties = filter.Key.Split('.'); // تقسيم الـ Key لمعرفة إن كان يحتوي على علاقة
                    var parameter = Expression.Parameter(typeof(T), "x");
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

                        var lambda = Expression.Lambda<Func<T, bool>>(containsExpression, parameter);
                        query = query.Where(lambda);
                    }
                    else if (propertyAccess.Type == typeof(DateTime) || propertyAccess.Type == typeof(DateTime?))
                    {
                        if (DateTime.TryParse(filter.Value.ToString(), out DateTime dateValue))
                        {
                            var value = Expression.Constant(dateValue, typeof(DateTime));
                            var greaterThanOrEqualExpression = Expression.GreaterThanOrEqual(propertyAccess, value);
                            var lambda = Expression.Lambda<Func<T, bool>>(greaterThanOrEqualExpression, parameter);
                            query = query.Where(lambda);
                        }
                    }
                    else if (propertyAccess.Type == typeof(int) || propertyAccess.Type == typeof(int?))
                    {
                        if (int.TryParse(filter.Value.ToString(), out int intValue))
                        {
                            var value = Expression.Constant(intValue);
                            var equalExpression = Expression.Equal(propertyAccess, value);
                            var lambda = Expression.Lambda<Func<T, bool>>(equalExpression, parameter);
                            query = query.Where(lambda);
                        }
                    }
                    else if (propertyAccess.Type == typeof(double) || propertyAccess.Type == typeof(double?))
                    {
                        if (double.TryParse(filter.Value.ToString(), out double doubleValue))
                        {
                            var value = Expression.Constant(doubleValue);
                            var equalExpression = Expression.Equal(propertyAccess, value);
                            var lambda = Expression.Lambda<Func<T, bool>>(equalExpression, parameter);
                            query = query.Where(lambda);
                        }
                    }
                    else if (propertyAccess.Type == typeof(bool) || propertyAccess.Type == typeof(bool?))
                    {
                        if (bool.TryParse(filter.Value.ToString(), out bool boolValue))
                        {
                            var value = Expression.Constant(boolValue);
                            var equalExpression = Expression.Equal(propertyAccess, value);
                            var lambda = Expression.Lambda<Func<T, bool>>(equalExpression, parameter);
                            query = query.Where(lambda);
                        }
                    }
                }
            }
            res.Total = query.Count();
            res.Data = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            // تطبيق الـ Pagination
            return res ;
        }

        public T? GetOne(Expression<Func<T, bool>> expression, Expression<Func<T, object>>[]? includes = null
            ,
            Func<IQueryable<T>, IQueryable<T>>? additionalIncludes = null)
        {
            var query = _model.AsQueryable();
            if (query != null)
            {
                if (includes != null && includes.Length > 0)
                {
                    foreach (var include in includes)
                    {
                        query = query.Include(include);
                    }
                }
                if (additionalIncludes != null)
                {
                    query = additionalIncludes(query);
                }
            }
            return query?.FirstOrDefault(expression);
        }
        public void Create(T item)
        {
            _model.Add(item);
        }
        public void Edit(T entity)
        {
            _model.Update(entity);
        }
        public void commit()
        {
            _context.SaveChanges();
        }
        public bool Delete(int id)
        {
            var item = _model.Find(id);
            if (item != null)
            {
                _model.Remove(item);
                return true;
            }
            return false;
        }
    }
}
