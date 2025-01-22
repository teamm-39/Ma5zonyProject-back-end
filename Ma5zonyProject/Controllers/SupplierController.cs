using DataAccess.IRepos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ma5zonyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private SupplierIRepo _supplier;

        public SupplierController(SupplierIRepo supplier)
        {
            _supplier = supplier;
        }
    }
