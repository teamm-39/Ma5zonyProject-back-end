using DataAccess.IRepos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using Models.ViewModels;
using Utility;

namespace Ma5zonyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationUserIRepo _users;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(ApplicationUserIRepo users,
                        UserManager<ApplicationUser> userManager,
                        RoleManager<IdentityRole> roleManager)
        {
            _users = users;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllAdmins(
            int pageSize = 5,
            int pageNumber = 1
            , string? name = null
            , string? userName = null
            , int? age = null
            , string? phone = null
            , string? address = null)
        {
            var res = new Result<List<AdminsDTO>>(isSuccess: false, message: "",
                                              pageNumber: pageNumber, pageSize: pageSize, data: []);
            if (pageSize < 1 || pageNumber < 1)
            {
                res.Meesage = "رقم الصفحة وعدد العناصر يجب أن يكونا أكبر";
                return BadRequest(res);
            }
            var adminUser = await _userManager.GetUsersInRoleAsync("Admin");
            var adminIds = adminUser.Select(u => u.Id).ToList();
            //fiteration
            var filter = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(name)) filter.Add("Name", name);
            if (!string.IsNullOrEmpty(userName)) filter.Add("UserName", userName);
            if (age.HasValue) filter.Add("Age", age);
            if (!string.IsNullOrEmpty(phone)) filter.Add("PhoneNumber", phone);
            if (!string.IsNullOrEmpty(address)) filter.Add("Address", address);
            //data
            var adminUsers = _users.GetAll(
                                            pageNumber: pageNumber,
                                            pageSize: pageSize,
                                            filters: filter,
                                            expression: e => adminIds.Contains(e.Id)
                                        )?.ToList() ?? new List<ApplicationUser>();
            res.IsSuccess = true;

            res.Data = adminUsers.Select(user => new AdminsDTO
            {
                Id = user.Id,
                Address = user.Address,
                Age = user.Age,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Name = user.Name,
                UserName = user.UserName,
                ImgUrl = user.ImgUrl
            }).ToList();

            return Ok(res);
        }
    }
}
