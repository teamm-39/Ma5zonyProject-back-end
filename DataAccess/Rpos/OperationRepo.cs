using DataAccess.Data;
using DataAccess.IRepos;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        public static List<string> MonthsArabicNames = new List<string>
{
    "يناير", "فبراير", "مارس", "أبريل", "مايو", "يونيو", "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر"
};
        public OperationRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public int CreateOperation(int LkOperationType, string userId, int supplierOrCustomerId, double totalPrice)
        {
            var operation = new Operation() { ApplicationUserId = userId, DateTime = DateTime.Now, LookupOperationTypeId = LkOperationType, TotalPrice = totalPrice };
            if (LkOperationType == StaticData.ImportOperation)
            {
                operation.CustomerSupplierId = supplierOrCustomerId;
            }
            else if (LkOperationType == StaticData.ExportOperation)
            {
                operation.CustomerSupplierId = supplierOrCustomerId;
            }
            _context.Add(operation);
            _context.SaveChanges();
            return operation.OperationId;
        }
        public List<TotalPriceInMonth> GetTotalInAlMonths(
int year,int operationType)
        {
            var data = _context.Operations.Where(e => e.DateTime.Year == year&&e.LookupOperationTypeId==operationType).GroupBy(e => new { e.DateTime.Year, e.DateTime.Month }).ToList()
                .Select(e => new TotalPriceInMonth
                {
                    MonthName = CultureInfo.GetCultureInfo("ar-EG").DateTimeFormat.GetMonthName(e.Key.Month),
                    TotalPrice = e.Sum(x => x.TotalPrice),
                    MonthNumber = e.Key.Month
                }).OrderBy(x => x.MonthNumber)
        .ToList();

                var allMonthsData = MonthsArabicNames
        .Select((monthName, index) => new TotalPriceInMonth
        {
            MonthName = monthName,
            MonthNumber = index + 1,
            TotalPrice = data.FirstOrDefault(x => x.MonthNumber == index + 1)?.TotalPrice ?? 0
        })
        .OrderBy(x => x.MonthNumber)
        .ToList();
            return allMonthsData;
        }
    }
}
