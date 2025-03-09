using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.Models;
using Utility;
using Models.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.EntityFrameworkCore;

namespace Ma5zonyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<ApplicationUser> userManger, SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManger;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        [HttpGet]
        public async Task<ActionResult> GetAllAdmins(int pageSize = 5, int pageNumber = 1, string? name = null, string? userName = null, int? age = null
            , string? phone = null, string? address = null)
        {
            if (pageNumber > 0 && pageSize > 0)
            {
                var users = _userManager.Users.AsQueryable();
                var adminUsers = new List<AdminsDTO>();

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
                        var adminUser = new AdminsDTO()
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
                var result = new Result<List<AdminsDTO>>(true, total, pageSize, pageNumber, adminUsers);
                return Ok(result);
            }
            var invalidResult = new Result<List<AdminsDTO>>(false, 0, pageSize, pageNumber, [], "يجب ان يكون رقم الصفحه وعدد العناصر اكبر من الصفر");
            return BadRequest(invalidResult);
        }
        [HttpPost("sign-up")]
        public async Task<ActionResult> Register(UserRegisterVM user)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser newUser = new()
                {
                    UserName = user.UserName,
                    Name = user.Name,
                    Email = user.Email,
                    Age = user.Age,
                };
                var result = await _userManager.CreateAsync(newUser, user.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newUser, "admin");
                    await _signInManager.SignInAsync(newUser, isPersistent: false);
                    var outbut = new Result<UserRegisterVM>(isSuccess: true, total: 0, 0, 0, data: user);
                    return Ok(outbut);
                }
                var e = result.Errors.Select(e => e.Description).ToList();
                foreach (var error in e)
                {
                    Console.WriteLine(error);
                }
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                               .Select(e => e.ErrorMessage)
                               .ToList();
            foreach (var error in errors)
            {
                Console.WriteLine(error);
            }
            return BadRequest(user);
        }
        [HttpPost("sign-in")]
        public async Task<ActionResult> Login(UserLoginVM user)
        {
            var xx = User.Identity.IsAuthenticated;
            Result<UserLoginVM> result = new(false);
            if (ModelState.IsValid)
            {
                var applicationUser = await _userManager.FindByEmailAsync(user.Email);
                if (applicationUser != null && await _userManager.CheckPasswordAsync(applicationUser, user.Password))
                {
                    await _signInManager.SignInAsync(applicationUser,isPersistent: true);
                    result.IsSuccess = true;
                    return Ok(result);
                }
                result.Meesage = "Invalid email or password";
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpGet("log-out")]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            var result = new Result<Array>(isSuccess: true);
            return Ok(result);
        }
        [HttpGet("is-loged-in")]
        public IActionResult Chek()
        {
            var result = new Result<bool>();
            result.IsSuccess = true;
            if (User.Identity.IsAuthenticated)
            {
                result.Data = true;
                return Ok(result);
            }
            else
            {
                result.Data = false;
                return Ok(result);
            }
        }
    }
}
