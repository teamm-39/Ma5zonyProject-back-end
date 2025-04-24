using DataAccess.IRepos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Models.ViewModels;
using System.Security.Claims;
using System;
using Utility;
using System.Linq.Expressions;

namespace Ma5zonyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerLogController : ControllerBase
    {
        private CustomerSupplierLogIRepo _log;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomerLogController(CustomerSupplierLogIRepo log, UserManager<ApplicationUser> userManager)
        {
            _log = log;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int pageSize = 5, int pageNumber = 1,
            DateTime? FromDateTime = null, DateTime? ToDateTime = null,
            string? oldName = null, string newName = null,
            string? oldEmail = null, string? newEmail = null,
            string? oldPhoneNumber = null, string? newPhoneNumber = null,
            string? userName=null, int? operationType = null)
        {
            var res=new Result<List<CustomerSupplierLogVM>>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted == true)
            {
                res.Meesage = "يرجى تسجيل الدخول اولا";
                return Unauthorized(res);
            }
            if (pageNumber <= 0 || pageSize <= 0)
            {
                res.Meesage = "رقم الصفحة وعدد العناصر يجب أن يكونا أكبر من الصفر";
                return BadRequest(res);
            }
            if (FromDateTime > DateTime.Now || ToDateTime > DateTime.Now)
            {
                res.Meesage = "التاريخ يجب ان يكون بحد اقصى اليوم";
                return BadRequest(res);
            }
            var filters = new Dictionary<string, object>();

            if (!string.IsNullOrWhiteSpace(userName))
                filters.Add("ApplicationUser.Name", userName);

            if (operationType.HasValue)
            {
                filters.Add("LookupOperationTypeId", operationType);
            }
            if (!string.IsNullOrWhiteSpace(oldName))
            {
                filters.Add("OldName", oldName);
            }
            if (!string.IsNullOrWhiteSpace(newName))
            {
                filters.Add("NewName", newName);
            }
            if (!string.IsNullOrWhiteSpace(oldEmail))
            {
                filters.Add("OldEmail", oldEmail);
            }
            if (!string.IsNullOrWhiteSpace(newEmail))
            {
                filters.Add("NewEmail", newEmail);
            }
            if (!string.IsNullOrWhiteSpace(oldPhoneNumber))
            {
                filters.Add("OldPhoneNumber", oldPhoneNumber);
            }
            if (!string.IsNullOrWhiteSpace(newPhoneNumber))
            {
                filters.Add("NewPhoneNumber", newPhoneNumber);
            }
            Expression<Func<CustomerSupplierLog, bool>> dateFilter = null;
            var to = ToDateTime?.Date.AddDays(1);
            if (FromDateTime.HasValue && ToDateTime.HasValue)
            {
                dateFilter = log => log.DateTime >= FromDateTime.Value.Date && log.DateTime < to.Value;
            }
            else if (FromDateTime.HasValue)
            {
                dateFilter = log => log.DateTime >= FromDateTime.Value.Date;
            }
            else if (ToDateTime.HasValue)
            {
                dateFilter = log => log.DateTime < to.Value;
            }
            filters.Add("LookupCustomerSupplierTypeId",2);
            var data = _log.GetAll(pageSize: pageSize, pageNumber: pageNumber, filters: filters, expression: dateFilter, includes: [e=>e.ApplicationUser]);
            res.Data=data.Data?.Select(e=>new CustomerSupplierLogVM
            {
                CustomerSupplierLogId = e.CustomerSupplierLogId,
                DateTime = e.DateTime,
                LookupOperationTypeId = e.LookupOperationTypeId,
                Message = e.Message,
                NewAddress = e.NewAddress,
                NewAge = e.NewAge,
                NewEmail = e.NewEmail,
                NewIsReliable = e.NewIsReliable,
                NewPhoneNumber = e.NewPhoneNumber,
                NewName = e.NewName,
                OldAddress = e.OldAddress,
                OldAge = e.OldAge,
                OldEmail = e.OldEmail,
                OldIsReliable = e.OldIsReliable,
                OldName = e.OldName,
                OldPhoneNumber = e.OldPhoneNumber,
                UserName=e.ApplicationUser.Name
            }).ToList();
            res.Total = data.Total;
            res.PageNumber = pageNumber;
            res.PageSize=pageSize;
            res.IsSuccess = true;
            return Ok(res);
        }
    }
}
