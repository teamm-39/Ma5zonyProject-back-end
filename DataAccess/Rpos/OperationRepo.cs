using DataAccess.Data;
using DataAccess.IRepos;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DataAccess.Rpos
{
    public class OperationRepo : BaseRepo<Operation>, OperationIRepo
    {
        ApplicationDbContext _context;
        public OperationRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public int CreateOperation(int LkOperationType, string userId, int supplierOrCustomerId, double totalPrice)
        {
            var operation = new Operation() { ApplicationUserId = userId, DateTime = DateTime.Now, LookupOperationTypeId = LkOperationType, TotalPrice=totalPrice };
            if (LkOperationType == StaticData.ImportOperation)
            {
                operation.SupplierId= supplierOrCustomerId;
            }else if (LkOperationType == StaticData.ExportOperation)
            {
                operation.CustomerId= supplierOrCustomerId;
            }
            _context.Add(operation);
            _context.SaveChanges();
            return operation.OperationId;
        }

    }
}
