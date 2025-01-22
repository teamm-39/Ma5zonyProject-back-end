﻿using DataAccess.IRepos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ma5zonyProject.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserMangerStoreController : ControllerBase
    {
        private UserMangerStoreIRepo UserMangeStoreI;
        public UserMangerStoreController(UserMangerStoreIRepo userMangeStoreI)
        {
            _userMangeStore = userMangeStoreI;
        }
    }
}
