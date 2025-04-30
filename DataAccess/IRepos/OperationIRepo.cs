using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Models.Models;
using Models.ViewModels;

namespace DataAccess.IRepos
{
    public interface OperationIRepo : IBaseRepo <Operation>
    {
        public int CreateOperation(int LkOperationType, string userId, int supplierOrCustomerId,double totalPrice);
        public List<TotalPriceInMonth> GetTotalInAlMonths(
int year, int operationType);
    }
}
