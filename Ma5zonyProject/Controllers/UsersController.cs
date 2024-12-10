using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.Models;
using Models.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public async Task<ActionResult> GetAll()
        {
            var users=_userManager.Users.ToList();
            return Ok(users);
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
                var result= await _userManager.CreateAsync(newUser,user.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newUser, "admin");
                    await _signInManager.SignInAsync(newUser, isPersistent: false);
                    return Ok();
                }
                var e = result.Errors.Select(e => e.Description).ToList();
                foreach (var error in e)
                {
                    Console.WriteLine(error);  // أو يمكنك تسجيلها باستخدام Logger
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
    }
}
