using DataAccess.Data;
using DataAccess.IRepos;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DataAccess.Rpos
{
    public class StoreLogRepo:BaseRepo<StoreLog> , StoreLogIRepo
    {
        ApplicationDbContext _context;
        public StoreLogRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public void CreateOperationLog(Store? oldStore,Store? newStore,int operationType,string userId)
        {
            var storeLog = new StoreLog();
            if(operationType== StaticData.AddOperationType)
            {
                storeLog=new StoreLog{ ApplicationUserId = userId, OldName = "-", OldCountry = "-", OldCity = "-", NewName = newStore.Name, NewCountry = newStore.Country, NewCity = newStore.City,Message = "تم اضافة المخزن" };
            }else if (operationType==StaticData.EditOperationType)
            {
                storeLog = new StoreLog { ApplicationUserId = userId, OldName = oldStore.Name, OldCountry = oldStore.Country, OldCity = oldStore.City, NewName = newStore.Name, NewCountry = newStore.Country, NewCity = newStore.City, Message = "تم تعديل المخزن" };

            }else
            {
                storeLog = new StoreLog { ApplicationUserId = userId, OldName = oldStore.Name, OldCountry = oldStore.Country, OldCity = oldStore.City, NewName = "-", NewCountry = "-", NewCity = "-", Message = "تم حذف المخزن" };
            }
            storeLog.LookupOperationTypeId = operationType;
            _context.StoreLogs.Add(storeLog);
            _context.SaveChanges();
        }
    }
}
