using DataAccess.IRepos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ma5zonyProject.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ImportController : ControllerBase
    {
        private ImportIRepo _import;

        public ImportController(ImportIRepo import)
        {
            _import = import;
        }
    }
