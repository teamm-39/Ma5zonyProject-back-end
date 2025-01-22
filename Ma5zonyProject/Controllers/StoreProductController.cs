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
        private StoreProductIRepo StoreProduct;
        public StoreProductController(StoreProductIRepo storeProductI)
        {
            _StoreProduct = storeProductI;
        }
        }
    }
}
