using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.Models;
using Utility;
using Models.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
            Result<UserLoginVM> result = new(false);
            if (ModelState.IsValid)
            {
                var applicationUser = await _userManager.FindByEmailAsync(user.Email);
                if (applicationUser != null && await _userManager.CheckPasswordAsync(applicationUser, user.Password))
                {
                    await _signInManager.SignInAsync(applicationUser, isPersistent: true);
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
        public async Task<IActionResult> Chek()
        {
            var result = new Result<bool>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted == true)
            {
                result.Meesage = "يرجى تسجيل الدخول";
                result.Data = false;
                result.IsSuccess = true;
                return Unauthorized(result);
            }
            result.Data = true;
            return Ok(result);
        }
        [HttpGet("get-user-profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var res = new Result<UserProfileVM>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted == true)
            {
                res.Meesage = "يرجى تسجيل الدخول";
                return BadRequest(res);
            }
            var roles = await _userManager.GetRolesAsync(user);
            var roleName = roles.FirstOrDefault();
            res.Data = new UserProfileVM
            {
                UserName = user.UserName,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Age = user.Age,
                ImgUrl = user.ImgUrl,
                RoleName = roleName
            };
            res.IsSuccess = true;
            return Ok(res);
        }
        [HttpPut("update-user-profile")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> EditUserProfile([FromForm] UserProfileVM userProfile, IFormFile? img)
        {
            var res = new Result<UserProfileVM>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted == true)
            {
                res.Meesage = "لم يتم العثور على هذا المستخدم";
                return BadRequest(res);
            }
            user.Name = userProfile.Name;
            user.Email = userProfile.Email;
            user.PhoneNumber = userProfile.PhoneNumber;
            user.Address = userProfile.Address;
            user.Age = userProfile.Age;
            user.UserName = userProfile.UserName;
            if (userProfile.Password != null && userProfile.Password != userProfile.ConfirmPassword)
            {
                res.Meesage = "كلمة المرور وتأكيد كلمة المرور غير متطابقتين";
                return BadRequest(res);
            }
            if (!string.IsNullOrEmpty(userProfile.Password))
            {
                var removeOldPassword = await _userManager.RemovePasswordAsync(user);
                var addNewPassWord = await _userManager.AddPasswordAsync(user, userProfile.Password);
                if (!removeOldPassword.Succeeded || !addNewPassWord.Succeeded)
                {
                    res.Meesage = "حدث خطأ اثناء تغير كلمة المرور";
                    return BadRequest(res);
                }
            }
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/profilePicture");
            if (img != null && img.Length > 0)
            {
                var allowedExtensions = new HashSet<string> { ".jpg", ".jpeg", ".png", ".bmp" };
                var imgExtension = Path.GetExtension(img.FileName).ToLower();

                if (!allowedExtensions.Contains(imgExtension))
                {
                    res.Meesage = "يُسمح فقط بتحميل الصور بامتدادات: JPG, JPEG, PNG, BMP.";
                    return BadRequest(res);
                }
                var imgEx = Path.GetExtension(img.FileName);

                if (!string.IsNullOrEmpty(user.ImgUrl))
                {
                    FileHelper.DeleteFile(folderPath, user.ImgUrl);
                }
                user.ImgUrl = Guid.NewGuid().ToString() + imgEx;
                await FileHelper.SaveFileAsync(img, folderPath, user.ImgUrl);
            }
            var updateUser= await _userManager.UpdateAsync(user);
            if (!updateUser.Succeeded)
            {
                res.Meesage = "حدث خطأ اثناء تحديث المستخدم";
                return BadRequest(res);
            }
            res.IsSuccess = true;
            return Ok(res);
        }
    }
}
