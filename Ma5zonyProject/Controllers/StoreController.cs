using DataAccess.IRepos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
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
            var res = new Result<List<Store>>(isSuccess: false, message: "Page number and page size must be greater than 0.", pageNumber: pageNumber, pageSize: pageSize, data: []);
            return BadRequest(res);
        }
    }
}
