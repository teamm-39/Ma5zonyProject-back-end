using DataAccess.IRepos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.Models;
using Models.ViewModels;
using System.Security.Claims;
using Utility;

namespace Ma5zonyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreLogsController : ControllerBase
    {
        private StoreLogIRepo _log;
        private readonly UserManager<ApplicationUser> _userManager;

        public StoreLogsController(StoreLogIRepo log, UserManager<ApplicationUser> userManager)
        {
            _log = log;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageSize = 5, int pageNumber = 1, DateTime? dateTime = null, string? storeName=null, string? userName=null, int? operationType=null)
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
            if (dateTime > DateTime.Now)
            {
                res.Meesage = "التاريخ يجب ان يكون بحد اقصى اليوم";
                return BadRequest(res);
            }

            var filters = new Dictionary<string, object>();

            if (!string.IsNullOrWhiteSpace(userName))
                filters.Add("ApplicationUser.Name", userName);

            if (dateTime.HasValue)
                filters.Add("DateTime", dateTime.Value);

            if (!string.IsNullOrEmpty(storeName))
            {
                filters.Add("OldName", storeName);
                filters.Add("NewName", storeName);
            }
            if (operationType.HasValue)
            {
                filters.Add("LookupOperationTypeId", operationType);
            }
            var log = _log.GetAll(pageSize: pageSize, pageNumber: pageNumber, includes: [e => e.ApplicationUser], filters: filters);
            res.Data = log.Data.Select(e => new StoreLogVM {
            
            }).ToList();
            return Ok(log);
        }
    }
}
