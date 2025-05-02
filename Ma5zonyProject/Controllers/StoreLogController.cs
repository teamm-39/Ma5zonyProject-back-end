using DataAccess.IRepos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.Models;
using Models.ViewModels;
using System;
using System.Linq.Expressions;
using System.Security.Claims;
using Utility;

namespace Ma5zonyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreLogController : ControllerBase
    {
        private StoreLogIRepo _log;
        private readonly UserManager<ApplicationUser> _userManager;

        public StoreLogController(StoreLogIRepo log, UserManager<ApplicationUser> userManager)
        {
            _log = log;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageSize = 5, int pageNumber = 1, DateTime? fromDateTime = null,
            DateTime? toDateTime = null, string? oldStoreName = null, string? newStoreName = null, string? userName = null, int? operationType = null)
        {
            var res = new Result<List<StoreLogVM>>();
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

            Expression<Func<StoreLog, bool>> dateFilter = null;
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

            if (operationType.HasValue)
            {
                filters.Add("LookupOperationTypeId", operationType);
            }

            if (!string.IsNullOrWhiteSpace(oldStoreName))
            {
                filters.Add("OldName", oldStoreName);
            }
            if (!string.IsNullOrWhiteSpace(newStoreName))
            {
                filters.Add("NewName", newStoreName);
            }
            var log = _log.GetAll(pageSize: pageSize, pageNumber: pageNumber, includes: [e => e.ApplicationUser], filters: filters,expression:dateFilter);
            res.Data = log.Data.Select(e => new StoreLogVM
            {
                LookupOperationTypeId = e.LookupOperationTypeId,
                Message = e.Message,
                NewCity = e.NewCity,
                NewCountry = e.NewCountry,
                DateTime = e.DateTime,
                NewName = e.NewName,
                OldCountry = e.OldCountry,
                OldName = e.OldName,
                OlgCity = e.OldCity,
                StoreLogId = e.StoreLogId,
                UserName = e.ApplicationUser.Name
            }).ToList();
            res.IsSuccess = true;
            res.Total = log.Total;
            res.PageNumber = pageNumber;
            res.PageSize = pageSize;
            return Ok(res);
        }
        [HttpGet("getAllWithoutPagination")]
        public async Task<IActionResult> GetAllWithoutPagination(string? oldStoreName, string? newStoreName, DateTime? fromDateTime = null,
            DateTime? toDateTime = null, string? userName = null, int? operationType = null)
        {
            var res = new Result<List<StoreLogVM>>();
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

            Expression<Func<StoreLog, bool>> dateFilter = null;
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

            if (operationType.HasValue)
            {
                filters.Add("LookupOperationTypeId", operationType);
            }
            if (!string.IsNullOrWhiteSpace(oldStoreName))
            {
                filters.Add("OldName", oldStoreName);
            }
            if (!string.IsNullOrWhiteSpace(newStoreName))
            {
                filters.Add("NewName", newStoreName);
            }
            res.Data = _log.GetAllWithoutPagination(includes: [e => e.ApplicationUser], filters: filters,expression:dateFilter);
            res.IsSuccess = true;
            res.Total = res.Data.Count;
            return Ok(res);
        }
    }
}
