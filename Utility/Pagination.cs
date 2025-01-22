using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class Pagination<T> where T : class
    {
        static public IEnumerable<T> Paginate(IEnumerable<T> data, int pageNum, int pageSize)
        {
            if (data != null)
            {
                    var skip = pageNum - 1 * pageSize;
                    return data.Skip(skip).Take(pageSize);
            }
            return null;
        }
    }
}
