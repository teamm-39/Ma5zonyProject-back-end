using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public int Total { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public T? Data { get; set; }
        public Result(bool isSuccess = false, int total = 0, int pageSize = 0, int pageNumner = 0, T? data = default )
        {
            IsSuccess = isSuccess;
            Total = total;
            PageSize = pageSize;
            PageNumber = pageNumner;
            Data = data;
        }
    }
}
