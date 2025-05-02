using DataAccess.IRepos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Models.ViewModels;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Utility;

namespace Ma5zonyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductLogController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ProductLogIRepo _log;

        public ProductLogController(UserManager<ApplicationUser> userManager, ProductLogIRepo log)
        {
            _userManager = userManager;
            _log = log;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageSize = 5,
            int pageNumber = 1,
            DateTime? fromDateTime = null,
            DateTime? toDateTime = null,
            string? oldProductName = null,
            string? newProductName = null,
            double? oldSellingPrice = null,
            double? newSellingPrice = null,
            double? oldPurchasePrice = null,
            double? newPurchasePrice = null,
            string? userName = null,
            int? operationType = null)
        {
            var res = new Result<List<ProductLogVM>>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted == true)
            {
                res.Meesage = "يرجى تسجيل الدخول اولا";
                return Unauthorized(res);
            }
            if (pageSize <= 0 || pageNumber <= 0)
            {
                res.Meesage = "رقم الصفحة وعدد العناصر يجب أن يكونا أكبر من الصفر";
                return BadRequest(res);
            }
            if (fromDateTime > DateTime.Now || toDateTime > DateTime.Now)
            {
                res.Meesage = "التاريخ يجب ان يكون بحد اقصى اليوم";
                return BadRequest(res);
            }
            if (oldSellingPrice <= 0 || newSellingPrice <= 0 || oldPurchasePrice <= 0 || newPurchasePrice <= 0)
            {
                res.Meesage = "سعر الشراء وسعر البيع يجب ان يكونوا اكبر من الصفر";
                return BadRequest(res);
            }
            var filters = new Dictionary<string, object>();

            if (!string.IsNullOrWhiteSpace(userName))
                filters.Add("ApplicationUser.Name", userName);

            Expression<Func<ProductLog, bool>> dateFilter = null;
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
                filters.Add("LookupOperationTypeId", operationType);

            if (!string.IsNullOrWhiteSpace(oldProductName))
                filters.Add("OldName", oldProductName);

            if (!string.IsNullOrWhiteSpace(newProductName))
                filters.Add("NewName", newProductName);

            if (oldSellingPrice.HasValue)
                filters.Add("OldSellingPrice", oldSellingPrice.Value);

            if (newSellingPrice.HasValue)
                filters.Add("NewSellingPrice", newSellingPrice.Value);

            if (oldPurchasePrice.HasValue)
                filters.Add("OldPurchasePrice", oldPurchasePrice.Value);

            if (newPurchasePrice.HasValue)
                filters.Add("NewPurchasePrice", newPurchasePrice.Value);

            var data = _log.GetAll(pageSize: pageSize, pageNumber: pageNumber, filters: filters, includes: [e => e.ApplicationUser],expression:dateFilter);
            res.IsSuccess = true;
            res.PageSize = pageSize;
            res.PageNumber = pageNumber;
            res.Total = data.Total;
            res.Data=data?.Data?.Select(e => new ProductLogVM
            {
                DateTime = e.DateTime,
                LookupOperationTypeId = e.LookupOperationTypeId,
                Message = e.Message,
                NewMinLimit = e.NewMinLimit,
                NewName = e.NewName,
                NewPurchasePrice = e.NewPurchasePrice,
                NewSellingPrice = e.NewSellingPrice,
                OldMinLimit = e.OldMinLimit,
                OldName = e.OldName,
                OldPurchasePrice = e.OldPurchasePrice,
                OldSellingPrice = e.OldSellingPrice,
                ProductLogId = e.ProductLogId,
                UserName = e.ApplicationUser.Name,
            }).ToList();
            return Ok(res);
        }
        [HttpGet("getAllWithoutPagination")]
        public async Task<IActionResult> GetAllWithoutPagination(
                                    DateTime? fromDateTime = null,
            DateTime? toDateTime = null,
            string? oldProductName = null,
            string? newProductName = null,
            double? oldSellingPrice = null,
            double? newSellingPrice = null,
            double? oldPurchasePrice = null,
            double? newPurchasePrice = null,
            string? userName = null,
            int? operationType = null)
        {
            var res = new Result<List<ProductLogVM>>();
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
            if (oldSellingPrice <= 0 || newSellingPrice <= 0 || oldPurchasePrice <= 0 || newPurchasePrice <= 0)
            {
                res.Meesage = "سعر الشراء وسعر البيع يجب ان يكونوا اكبر من الصفر";
                return BadRequest(res);
            }
            Expression<Func<ProductLog, bool>> dateFilter = null;
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

            var filters = new Dictionary<string, object>();

            if (!string.IsNullOrWhiteSpace(userName))
                filters.Add("ApplicationUser.Name", userName);


            if (operationType.HasValue)
                filters.Add("LookupOperationTypeId", operationType);

            if (!string.IsNullOrWhiteSpace(oldProductName))
                filters.Add("OldName", oldProductName);

            if (!string.IsNullOrWhiteSpace(newProductName))
                filters.Add("NewName", newProductName);

            if (oldSellingPrice.HasValue)
                filters.Add("OldSellingPrice", oldSellingPrice.Value);

            if (newSellingPrice.HasValue)
                filters.Add("NewSellingPrice", newSellingPrice.Value);

            if (oldPurchasePrice.HasValue)
                filters.Add("OldPurchasePrice", oldPurchasePrice.Value);

            if (newPurchasePrice.HasValue)
                filters.Add("NewPurchasePrice", newPurchasePrice.Value);
            var data = _log.GetAllWithoutPagination(includes: [e=>e.ApplicationUser],filters: filters,expression:dateFilter);
            res.Data= data;
            res.IsSuccess= true;
            res.Total = data.Count;
            return Ok(res);
        }
    }
}
