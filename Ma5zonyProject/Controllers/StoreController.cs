using DataAccess.IRepos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Models.ViewModels;
using Utility;

namespace Ma5zonyProject.Controllers
{
    [Authorize]
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
        public IActionResult GetAll(int pageSize = 10, int pageNumber = 1)
        {
            if (pageNumber > 0 && pageSize > 0)
            {
                var data = _store.GetAll();
                if (data != null)
                {
                    var total = data.Count();
                    var newData = Pagination<Store>.Paginate(data, pageNum: pageNumber, pageSize: pageSize).ToList();
                    var result = new Result<List<Store>>(isSuccess: true, total: total, pageSize: pageSize, pageNumber: pageNumber, data: newData);
                    return Ok(result);
                }
            }
            var res = new Result<List<Store>>(isSuccess: false, message: "رقم الصفحه و عدد العناصر يجب ان يكونوا اكبر من الصفر", pageNumber: pageNumber, pageSize: pageSize, data: []);
            return BadRequest(res);
        }
        [HttpPost]
        [Route("create")]
        public IActionResult Create(StoreVM storeVM)
        {
            var result = new Result<StoreVM>();
            if (ModelState.IsValid)
            {
                Store newStore = new Store { Name = storeVM.Name, Country = storeVM.Country, City = storeVM.City };
                _store.Create(newStore);
                _store.commit();
                result.IsSuccess = true;
                result.Data = storeVM;
                result.Meesage = "تم انشاء المخزن بنجاح";
                return Ok(result);
            }
            result.Data = storeVM;
            return BadRequest(result);
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
            result.Meesage = "لم يتم العثور على هذا العنصر";
            return Ok(result);
        }
    }
}
