using DataAccess.IRepos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Models.ViewModels;
using System.Security.Claims;
using Utility;
namespace Ma5zonyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImortOperationController : ControllerBase
    {
        private OperationIRepo _operation;
        public ImortOperationController(OperationIRepo operation)
        {
            _operation = operation;
        }
        [HttpGet]
        public IActionResult GetAll(int pageNumber = 1, int pageSize = 5)
        {

            return Ok();
        }
        [HttpPost("create")]
        public IActionResult Create(OperationStoreProductCreateVM operationStoreProductCreateVM)
        {
            var res = new Result<Operation>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) {
                res.Meesage = "يرجى تسجيل الدخول اولا";
                return BadRequest(res);
            }
            var operationId=_operation.CreateOperation(StaticData.ImportOperation, userId);
            _operation.commit();
            res.IsSuccess = true;
            return Ok(res);
        }
    }
}
