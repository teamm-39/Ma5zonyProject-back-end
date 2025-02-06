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
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllAdmins(int pageSize = 5, int pageNumber = 1, string? name = null, string? userName = null, int? age = null
    , string? phone = null, string? address = null)
        {
            if (pageNumber > 0 && pageSize > 0)
            {
                var users = _userManager.Users.AsQueryable();
                var adminUsers = new List<UserDTO>();

                if (!string.IsNullOrEmpty(name))
                {
                    users = users.Where(e => EF.Functions.Like(e.Name, $"%{name}%"));
                }
                if (!string.IsNullOrEmpty(userName))
                {
                    users = users.Where(e => EF.Functions.Like(e.UserName, $"%{userName}%"));
                }
                if (age.HasValue)
                {
                    users = users.Where(e => e.Age == age);
                }
                if (!string.IsNullOrEmpty(phone))
                {
                    users = users.Where(e => EF.Functions.Like(e.PhoneNumber, $"%{phone}%"));
                }
                if (!string.IsNullOrEmpty(address))
                {
                    users = users.Where(e => EF.Functions.Like(e.Address, $"%{address}%"));
                }
                var usersList = await users.AsNoTracking().ToListAsync();
                foreach (var user in usersList)
                {
                    var role = await _userManager.GetRolesAsync(user);
                    if (role.Contains(StaticData.admin))
                    {
                        var adminUser = new UserDTO()
                        {
                            Id = user.Id,
                            Address = user.Address,
                            Email = user.Email,
                            Name = user.Name,
                            UserName = user.UserName,
                            ImgUrl = user.ImgUrl,
                            PhoneNumber = user.PhoneNumber
                        };
                        adminUsers.Add(adminUser);
                    }
                }
                var total = adminUsers.Count();
                var result = new Result<List<UserDTO>>(true, total, pageSize, pageNumber, adminUsers);
                return Ok(result);
            }
            var invalidResult = new Result<List<UserDTO>>(false, 0, pageSize, pageNumber, [], "يجب ان يكون رقم الصفحه وعدد العناصر اكبر من الصفر");
            return BadRequest(invalidResult);
        }
    }
}
