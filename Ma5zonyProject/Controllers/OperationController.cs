using DataAccess.IRepos;
using DataAccess.Rpos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ma5zonyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationController : ControllerBase
    {
        private OperationIRepo _operation;

        public OperationController(OperationIRepo operation)
        {
            _operation = operation;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok();
        }
    }
}