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
    public class CustomerSupplierLogRepo : BaseRepo<CustomerSupplierLog>, CustomerSupplierLogIRepo
    {
        ApplicationDbContext _context;
        public CustomerSupplierLogRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public void CreateOperationLog(CustomerSupplier? oldCustomerSupplier, CustomerSupplier? newCustomerSupplier, int operationType, string userId)
        {
            var log = new CustomerSupplierLog();
            if (operationType == StaticData.AddOperationType)
            {
                log = new CustomerSupplierLog
                {
                    ApplicationUserId = userId,
                    LookupOperationTypeId = operationType,
                    LookupCustomerSupplierTypeId = newCustomerSupplier.LookupCustomerSupplierTypeId,
                    Message = newCustomerSupplier.LookupCustomerSupplierTypeId == 1 ? "تم اضافة المورد" : "تم اضافة العميل",
                    OldAddress = "-",
                    OldAge = 0,
                    OldEmail = "-",
                    OldIsReliable = false,
                    OldName = "-",
                    OldPhoneNumber = "-",
                    NewAddress = newCustomerSupplier.Address,
                    NewAge = newCustomerSupplier.Age,
                    NewEmail = newCustomerSupplier.Email,
                    NewIsReliable = newCustomerSupplier.IsReliable,
                    NewName = newCustomerSupplier.Name,
                    NewPhoneNumber = newCustomerSupplier.PhoneNumber
                };
            }
            else if (operationType == StaticData.EditOperationType)
            {
                log = new CustomerSupplierLog
                {
                    ApplicationUserId = userId,
                    LookupOperationTypeId = operationType,
                    LookupCustomerSupplierTypeId = oldCustomerSupplier.LookupCustomerSupplierTypeId,
                    Message = oldCustomerSupplier.LookupCustomerSupplierTypeId == 1 ? "تم تعديل المورد" : "تم تعديل العميل",
                    OldAddress = oldCustomerSupplier.Address,
                    OldAge = oldCustomerSupplier.Age,
                    OldEmail = oldCustomerSupplier.Email,
                    OldIsReliable = oldCustomerSupplier.IsReliable,
                    OldName = oldCustomerSupplier.Name,
                    OldPhoneNumber = oldCustomerSupplier.PhoneNumber,
                    NewAddress = newCustomerSupplier.Address,
                    NewAge = newCustomerSupplier.Age,
                    NewEmail = newCustomerSupplier.Email,
                    NewIsReliable = newCustomerSupplier.IsReliable,
                    NewName = newCustomerSupplier.Name,
                    NewPhoneNumber = newCustomerSupplier.PhoneNumber
                };
            }
            else
            {
                log = new CustomerSupplierLog
                {
                    ApplicationUserId = userId,
                    LookupOperationTypeId = operationType,
                    LookupCustomerSupplierTypeId = oldCustomerSupplier.LookupCustomerSupplierTypeId,
                    Message = oldCustomerSupplier.LookupCustomerSupplierTypeId == 1 ? "تم حذف المورد" : "تم حذف العميل",
                    NewAddress = "-",
                    NewAge = 0,
                    NewEmail = "-",
                    NewIsReliable = false,
                    NewName = "-",
                    NewPhoneNumber = "-",
                    OldAddress = oldCustomerSupplier.Address,
                    OldAge = oldCustomerSupplier.Age,
                    OldEmail = oldCustomerSupplier.Email,
                    OldIsReliable = oldCustomerSupplier.IsReliable,
                    OldName = oldCustomerSupplier.Name,
                    OldPhoneNumber = oldCustomerSupplier.PhoneNumber
                };
            }
            log.DateTime = DateTime.Now;
            _context.CustomerSupplierLogs.Add(log);
            _context.SaveChanges();
        }
    }
}
