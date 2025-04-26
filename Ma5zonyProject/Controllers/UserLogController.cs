using DataAccess.IRepos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Models.ViewModels;
using System.Linq.Expressions;
using System.Security.Claims;
using Utility;

namespace Ma5zonyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserLogController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private ApplicationUserLogIRepo _log;

        public UserLogController(UserManager<ApplicationUser> userManager, ApplicationUserLogIRepo log)
        {
            _userManager = userManager;
            _log = log;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(
    int pageSize = 5, int pageNumber = 1,
    DateTime? fromDateTime = null, DateTime? toDateTime = null,
    string? userName = null, int? operationType = null,
    string? oldUserName = null, string? newUserName = null,
    string? oldName = null, string? newName = null,
    int? oldAge = null, int? newAge = null,
    string? oldPhoneNumber = null, string? newPhoneNumber = null,
    string? oldAddress = null, string? newAddress = null)
        {
            var res = new Result<List<ApplicationUserLogVM>>();
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
            if (fromDateTime > DateTime.Now || toDateTime > DateTime.Now)
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
            if (!string.IsNullOrWhiteSpace(oldUserName))
                filters.Add("OldUserName", oldUserName);

            if (!string.IsNullOrWhiteSpace(newUserName))
                filters.Add("NewUserName", newUserName);

            if (!string.IsNullOrWhiteSpace(oldName))
                filters.Add("OldName", oldName);

            if (!string.IsNullOrWhiteSpace(newName))
                filters.Add("NewName", newName);

            if (!string.IsNullOrWhiteSpace(oldPhoneNumber))
                filters.Add("OldPhoneNumber", oldPhoneNumber);

            if (!string.IsNullOrWhiteSpace(newPhoneNumber))
                filters.Add("NewPhoneNumber", newPhoneNumber);

            if (!string.IsNullOrWhiteSpace(oldAddress))
                filters.Add("OldAddress", oldAddress);

            if (!string.IsNullOrWhiteSpace(newAddress))
                filters.Add("NewAddress", newAddress);

            if (oldAge.HasValue)
                filters.Add("OldAge", oldAge);
            if (newAge.HasValue)
                filters.Add("NewAge", newAge);

            Expression<Func<ApplicationUserLog, bool>> dateFilter = null;
            var to = toDateTime?.Date.AddDays(1);
            if (fromDateTime.HasValue && toDateTime.HasValue)
            {
                dateFilter = log => log.DateTime >= fromDateTime.Value.Date && log.DateTime < to.Value;
            }
            else if (fromDateTime.HasValue)
            {
                dateFilter = log => log.DateTime >= fromDateTime.Value.Date;
            }
            else if (toDateTime.HasValue)
            {
                dateFilter = log => log.DateTime < to.Value;
            }
            filters.Add("RoleName", StaticData.user);
            var data = _log.GetAll(pageSize: pageSize, pageNumber: pageNumber, filters: filters, expression: dateFilter, includes: [e => e.ApplicationUser]);
            res.Data = data.Data?.Select(e => new ApplicationUserLogVM
            {
                ApplicationUserLogId = e.ApplicationUserLogId,
                DateTime = e.DateTime,
                LookupOperationTypeId = e.LookupOperationTypeId,
                Message = e.Message,
                NewAddress = e.NewAddress,
                NewAge = e.NewAge,
                NewEmail = e.NewEmail,
                NewImgUrl = e.NewImgUrl,
                NewName = e.NewName,
                NewPhoneNumber = e.NewPhoneNumber,
                NewUserName = e.NewUserName,
                OldAddress = e.OldAddress,
                OldAge = e.OldAge,
                OldEmail = e.OldEmail,
                OldImgUrl = e.OldImgUrl,
                OldName = e.OldName,
                OldPhoneNumber = e.OldPhoneNumber,
                OldUserName = e.OldUserName,
                UserName = e.ApplicationUser.Name,
            }).ToList();
            res.PageNumber = pageSize;
            res.PageSize = pageSize;
            res.Total = data.Total;
            res.IsSuccess = true;
            return Ok(res);
        }
        [HttpGet("getAllWithoutPagination")]
        public async Task<IActionResult> GetAllWithoutPagination(
                DateTime? fromDateTime = null, DateTime? toDateTime = null,
    string? userName = null, int? operationType = null,
    string? oldUserName = null, string? newUserName = null,
    string? oldName = null, string? newName = null,
    int? oldAge = null, int? newAge = null,
    string? oldPhoneNumber = null, string? newPhoneNumber = null,
    string? oldAddress = null, string? newAddress = null
    )
        {
            var res = new Result<List<ApplicationUserLogVM>>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted == true)
            {
                res.Meesage = "يرجى تسجيل الدخول اولا";
                return Unauthorized(res);
            }
            if (fromDateTime > DateTime.Now || toDateTime > DateTime.Now)
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
            if (!string.IsNullOrWhiteSpace(oldUserName))
                filters.Add("OldUserName", oldUserName);

            if (!string.IsNullOrWhiteSpace(newUserName))
                filters.Add("NewUserName", newUserName);

            if (!string.IsNullOrWhiteSpace(oldName))
                filters.Add("OldName", oldName);

            if (!string.IsNullOrWhiteSpace(newName))
                filters.Add("NewName", newName);

            if (!string.IsNullOrWhiteSpace(oldPhoneNumber))
                filters.Add("OldPhoneNumber", oldPhoneNumber);

            if (!string.IsNullOrWhiteSpace(newPhoneNumber))
                filters.Add("NewPhoneNumber", newPhoneNumber);

            if (!string.IsNullOrWhiteSpace(oldAddress))
                filters.Add("OldAddress", oldAddress);

            if (!string.IsNullOrWhiteSpace(newAddress))
                filters.Add("NewAddress", newAddress);

            if (oldAge.HasValue)
                filters.Add("OldAge", oldAge);
            if (newAge.HasValue)
                filters.Add("NewAge", newAge);

            Expression<Func<ApplicationUserLog, bool>> dateFilter = null;
            var to = toDateTime?.Date.AddDays(1);
            if (fromDateTime.HasValue && toDateTime.HasValue)
            {
                dateFilter = log => log.DateTime >= fromDateTime.Value.Date && log.DateTime < to.Value;
            }
            else if (fromDateTime.HasValue)
            {
                dateFilter = log => log.DateTime >= fromDateTime.Value.Date;
            }
            else if (toDateTime.HasValue)
            {
                dateFilter = log => log.DateTime < to.Value;
            }
            filters.Add("RoleName", StaticData.user);
            res.Data = _log.GetAllWithoutPagination(filters: filters, includes: [e => e.ApplicationUser], expression: dateFilter);
            res.IsSuccess = true;
            res.Total = res.Data.Count;
            return Ok(res);
        }
    }
}
