using DataAccess.IRepos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ma5zonyProject.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductImportController : ControllerBase
    {
        private ProductImportIRepo _ProductImport;
            public ProductImportController(ProductImportIRepo productImportI)
        {
            _ProductImport = productImportI; 
        }
    }
}
