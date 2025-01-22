using DataAccess.IRepos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ma5zonyProject.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductExportController : ControllerBase
    {
        private ProductExportIRepo _productexport;
        public ProductExportController(ProductExportIRepo productExport)
        {
            _productexport = productExport;
        }



    }
}
