using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepos
{
    public interface IBaseRepo<T> where T : class
    {
        IEnumerable<T> GetAll(Expression<Func<T, object>>[]? includes = null,
            Expression<Func<T, bool>>? expression = null,
            Func<IQueryable<T>, IQueryable<T>>? additionalIncludes = null
            );
        public T? GetOne(
    Expression<Func<T, bool>> expression,
    Expression<Func<T, object>>[]? includes = null,
            Func<IQueryable<T>, IQueryable<T>>? additionalIncludes = null);
        public void Create(T item);
        public bool Delete(int id);
        public void commit();
    }
}
