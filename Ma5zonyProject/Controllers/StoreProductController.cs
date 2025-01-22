using DataAccess.IRepos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ma5zonyProject.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StoreProductController : ControllerBase
    {
        private StoreProductIRepo _storeProduct;
        public StoreProductController(StoreProductIRepo storeProductI)
        {
            _storeProduct = storeProductI;
        }
    }
}