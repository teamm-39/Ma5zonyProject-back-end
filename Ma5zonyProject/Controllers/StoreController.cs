using DataAccess.IRepos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.Models;
using Models.ViewModels;
using Utility;

namespace Ma5zonyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private StoreIRepo _store;

        public StoreController(StoreIRepo store)
        {
            _store = store;
        }
        [HttpGet]
        public IActionResult GetAll(int pageSize = 5, int pageNumber = 1, string? name = null, string? country = null, string? city = null)
        {
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
            var data = _store.GetAll(pageSize: pageSize, pageNumber: pageNumber, filters: filters);

            var total = data.Total;
            var result = new Result<List<Store>>(isSuccess: true, total: total, pageSize: pageSize, pageNumber: pageNumber, data: data.Data?.ToList());

            return Ok(result);
        }

        [HttpGet]
        [Route("get-store/{id}")]
        public IActionResult GetOne(int id)
        {
            var result = new Result<Store>();
            var store = _store.GetOne(e => e.StoreId == id);
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
        public IActionResult Create(StoreCreateVM storeVM)
        {
            var result = new Result<StoreCreateVM>();
            if (ModelState.IsValid)
            {
                Store newStore = new Store { Name = storeVM.Name, Country = storeVM.Country, City = storeVM.City };
                _store.Create(newStore);
                _store.commit();
                result.IsSuccess = true;
                result.Meesage = "تم انشاء المخزن بنجاح";
                return Ok(result);
            }
            result.Data = storeVM;
            return BadRequest(result);
        }
        [HttpPut]
        [Route("Edit")]
        public IActionResult Edit(StoreEditVM store)
        {
                var result = new Result<Store>();
            if (ModelState.IsValid)
            {
                var oldStore = _store.GetOne(e => e.StoreId == store.StoreId);
                if (oldStore != null)
                {
                    oldStore.Name= store.Name;
                    oldStore.Country= store.Country;
                    oldStore.City= store.City;
                    _store.Edit(oldStore);
                    _store.commit();
                    result.IsSuccess = true;
                    result.Meesage = "تم التعديل بنجاح";
                    return Ok(result);
                }
                result.Meesage = "لم يتم العثور على هذا العنصر";
                return Ok(result);
            }
            return BadRequest();
        }
        [HttpDelete]
        [Route("delete/{id}")]
        public IActionResult Delete(int id)
        {
            var result = new Result<Store>();
            var store=_store.GetOne(e=>e.StoreId == id);
            if (store != null)
            {
                _store.Delete(id);
                _store.commit();
                result.Meesage = "تم الحذف بنجاح";
                result.IsSuccess=true;
                return Ok(result);
            }
            result.Meesage = "لم يتم العثور على هذا العنصر";
            return Ok(result);
        }
    }
}
