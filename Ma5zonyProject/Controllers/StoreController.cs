using DataAccess.IRepos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
    public class StoreController : ControllerBase
    {
        private StoreIRepo _store;
        private StoreLogIRepo _log;
        private readonly UserManager<ApplicationUser> _userManager;

        public StoreController(StoreIRepo store, StoreLogIRepo log, UserManager<ApplicationUser> userManager)
        {
            _store = store;
            _log = log;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageSize = 5, int pageNumber = 1, string? name = null, string? country = null, string? city = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted == true)
            {
                var res = new Result<Store>();
                res.Meesage = "يرجى تسجيل الدخول اولا";
                return Unauthorized(res);
            }
            if (pageNumber <= 0 || pageSize <= 0)
            {
                var res = new Result<List<Store>>(isSuccess: false, message: "رقم الصفحة وعدد العناصر يجب أن يكونا أكبر من الصفر",
                                                  pageNumber: pageNumber, pageSize: pageSize, data: []);
                return BadRequest(res);
            }

            // إنشاء Dictionary لتخزين الفلاتر
            var filters = new Dictionary<string, object>();
            if (!string.IsNullOrWhiteSpace(name)) filters.Add("Name", name);
            if (!string.IsNullOrWhiteSpace(country)) filters.Add("Country", country);
            if (!string.IsNullOrWhiteSpace(city)) filters.Add("City", city);

            // استدعاء الفلترة والباجينيشن من الـ Repo مباشرة
            var data = _store.GetAll(pageSize: pageSize, pageNumber: pageNumber, filters: filters,expression:e=>e.IsDeleted==false);

            var total = data.Total;
            var result = new Result<List<Store>>(isSuccess: true, total: total, pageSize: pageSize, pageNumber: pageNumber, data: data.Data?.ToList());

            return Ok(result);
        }

        [HttpGet]
        [Route("get-store/{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var result = new Result<Store>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted == true)
            {
                result.Meesage = "يرجى تسجيل الدخول اولا";
                return Unauthorized(result);
            }
            var store = _store.GetOne(e => e.StoreId == id&&e.IsDeleted==false);
            if (store != null)
            {
                result.IsSuccess = true;
                result.Data = store;
                return Ok(result);
            }
            result.IsSuccess = true;
            result.Meesage = "لم يتم العثور على هذا العنصر";
            return Ok(result);
        }
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(StoreCreateVM storeVM)
        {
            var result = new Result<StoreCreateVM>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted == true)
            {
                result.Meesage = "يرجى تسجيل الدخول اولا";
                return Unauthorized(result);
            }
            if (ModelState.IsValid)
            {
                Store newStore = new Store { Name = storeVM.Name, Country = storeVM.Country, City = storeVM.City };
                _store.Create(newStore);
                _store.commit();
                result.IsSuccess = true;
                result.Meesage = "تم انشاء المخزن بنجاح";
                _log.CreateOperationLog(oldStore: null, newStore: newStore, operationType: StaticData.AddOperationType, userId: userId);
                return Ok(result);
            }
            result.Data = storeVM;
            return BadRequest(result);
        }
        [HttpPut]
        [Route("Edit")]
        public async Task<IActionResult> Edit(StoreEditVM storeVM)
        {
            var result = new Result<Store>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted == true)
            {
                result.Meesage = "يرجى تسجيل الدخول اولا";
                return Unauthorized(result);
            }
            if (ModelState.IsValid)
            {
                var store = _store.GetOne(e => e.StoreId == storeVM.StoreId);
                var oldStore = new Store
                {
                    StoreId = store.StoreId,
                    Name = store.Name,
                    Country = store.Country,
                    City = store.City
                };
                if (store != null)
                {
                    store.Name = storeVM.Name;
                    store.Country = storeVM.Country;
                    store.City = storeVM.City;
                    
                    _store.Edit(store);
                    _store.commit();
                    result.IsSuccess = true;
                    result.Meesage = "تم التعديل بنجاح";
                    _log.CreateOperationLog(oldStore, store,StaticData.EditOperationType,userId);
                    return Ok(result);
                }
                result.Meesage = "لم يتم العثور على هذا العنصر";
                return Ok(result);
            }
            return BadRequest();
        }
        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = new Result<Store>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted == true)
            {
                result.Meesage = "يرجى تسجيل الدخول اولا";
                return Unauthorized(result);
            }
            var store = _store.GetOne(e => e.StoreId == id);
            if (store != null)
            {
                store.IsDeleted = true;
                _store.Edit(store);
                _store.commit();
                result.Meesage = "تم الحذف بنجاح";
                result.IsSuccess = true;
                _log.CreateOperationLog(oldStore:store,newStore:null,StaticData.DeleteOperationType,userId);
                return Ok(result);
            }
            result.Meesage = "لم يتم العثور على هذا العنصر";
            return BadRequest(result);
        }
        [HttpGet("get-stores-for-operation")]
        public async Task<IActionResult> GetStoresForOperation()
        {
            var res = new Result<List<StoreForOperation>>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted == true)
            {
                res.Meesage = "يرجى تسجيل الدخول اولا";
                return Unauthorized(res);
            }
            res.Data = _store.GetStoresForOperations();
            res.IsSuccess = true;
            return Ok(res);
        }
    }
}
