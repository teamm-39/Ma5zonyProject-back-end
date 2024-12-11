using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.Models;
using Utility;
using Models.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ma5zonyProject.Controllers
{
    [IgnoreAntiforgeryToken]
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
        public async Task<ActionResult> GetAll()
        {
            var users = _userManager.Users.ToList();
            var total = users.Count();
            var result = new Result<List<ApplicationUser>>(true, total, total, 1, users);
            return Ok(result);
        }
        [HttpPost]
        public async Task<ActionResult> Register(UserRegisterVM user)
        {
            if (_roleManager.Roles.IsNullOrEmpty())
            {
                await _roleManager.CreateAsync(new("admin"));
            }
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
            Result<UserLoginVM> result = new(false, data: user);
            if (ModelState.IsValid)
            {
                var applicationUser = await _userManager.FindByEmailAsync(user.Email);
                if (applicationUser != null&& await _userManager.CheckPasswordAsync(applicationUser, user.Password))
                {
                    await _signInManager.SignInAsync(applicationUser, false);
                    result.IsSuccess = true;
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest(result);
        }
        [HttpPost("log-out")]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            var result = new Result<Array>(isSuccess:true);
            return Ok(result);
        }
    }
}
