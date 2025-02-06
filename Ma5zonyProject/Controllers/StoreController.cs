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
        public IActionResult GetAll(int pageSize = 5, int pageNumber = 1,string name="",string country="",string city="")
        {
            if (pageNumber > 0 && pageSize > 0)
            {
                var data = _store.GetAll();
                if (data != null)
                {
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        data=data.Where(e=>e.Name.ToLower().Contains(name.ToLower()));
                    }
                    if (!string.IsNullOrWhiteSpace(country))
                    {
                        data=data.Where(e=>e.Country.ToLower().Contains(country.ToLower()));
                    }
                    if (!string.IsNullOrWhiteSpace(city))
                    {
                        data=data.Where(e=>e.City.ToLower().Contains(city.ToLower()));
                    }
                    var total = data.Count();
                    var newData = Pagination<Store>.Paginate(data, pageNum: pageNumber, pageSize: pageSize).ToList();
                    var result = new Result<List<Store>>(isSuccess: true, total: total, pageSize: pageSize, pageNumber: pageNumber, data: newData);
                    return Ok(result);
                }
            }
            var res = new Result<List<Store>>(isSuccess: false, message: "رقم الصفحه و عدد العناصر يجب ان يكونوا اكبر من الصفر", pageNumber: pageNumber, pageSize: pageSize, data: []);
            return Ok(res);
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
