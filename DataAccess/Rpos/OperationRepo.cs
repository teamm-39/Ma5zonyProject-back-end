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

namespace DataAccess.Rpos
{
    public class OperationRepo : BaseRepo<Operation>, OperationIRepo
    {
        ApplicationDbContext _context;
        public OperationRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public int? CreateOperation(int LkOperationType, string userId)
        {
            var operation = new Operation() { ApplicationUserId = userId, DateTime = DateTime.Now, LookupOperationTypeId = LkOperationType };
            _context.Add(operation);
            return operation.OperationId;
        }

    }
}
