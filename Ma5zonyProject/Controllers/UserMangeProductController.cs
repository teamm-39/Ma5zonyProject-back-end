using DataAccess.IRepos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ma5zonyProject.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserMangeProductController : ControllerBase
    {
        private UserMangerProductIRepo _userMangerProductI;
        public UserMangeProductController(UserMangerProductIRepo userMangerProductI0)
        {
            _userMangerProductI = userMangerProductI0;
        }
    }
}
