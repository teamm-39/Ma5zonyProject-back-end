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
        public IEnumerable<T> GetAll(Expression<Func<T, object>>[]? includes = null,
            Expression<Func<T, bool>>? expression = null,
            Func<IQueryable<T>, IQueryable<T>>? additionalIncludes = null)
        {
            var query = _model.AsQueryable();
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
            if (additionalIncludes != null)
            {
                query = additionalIncludes(query);
            }
            return query;
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
