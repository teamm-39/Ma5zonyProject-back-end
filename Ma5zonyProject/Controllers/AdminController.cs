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
        [HttpPost("create")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> CreateAdmin([FromForm] AdminDTO admin, IFormFile? img)
        {
            var res = new Result<AdminDTO>();
            if (!ModelState.IsValid)
            {
                res.Meesage = "يوجد خطأ فى البيانات التى تم ارسالها";
                return BadRequest(res);
            }
            var existingEmail = await _userManager.FindByEmailAsync(admin.Email);
            if (existingEmail != null)
            {
                res.Meesage = "هذا البريد الإلكتروني مستخدم بالفعل.";
                return BadRequest(res);
            }
            var existingUserName = await _userManager.FindByNameAsync(admin.UserName);
            if (existingUserName != null)
            {
                res.Meesage = "اسم المستخدم مستخدم بالفعل.";
                return BadRequest(res);
            }
            if (img != null)
            {
                // السماح فقط بامتدادات الصور
                var allowedExtensions = new HashSet<string> { ".jpg", ".jpeg", ".png", ".bmp" };
                var imgExtension = Path.GetExtension(img.FileName).ToLower();

                if (!allowedExtensions.Contains(imgExtension))
                {
                    res.Meesage = "يُسمح فقط بتحميل صور فقط";
                    return BadRequest(res);
                }
            }
            string? imgName = img != null ? Guid.NewGuid().ToString() + Path.GetExtension(img.FileName) : null; 
            ApplicationUser user = new()
            {
                Name = admin.Name,
                Email = admin.Email,
                Age = admin.Age,
                PhoneNumber = admin.PhoneNumber,
                Address = admin.Address,
                ImgUrl = imgName,
                UserName = admin.UserName,
            };
            var createAdmin = await _userManager.CreateAsync(user, admin.Password);
            if (createAdmin.Succeeded)
            {
                if (img != null)
                {
                    var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/profilePicture/admins");
                    await FileHelper.SaveFileAsync(img, folder, imgName);
                }
                res.IsSuccess = true;
                await _userManager.AddToRoleAsync(user, StaticData.admin);
                return Ok(res);
            }
            res.Meesage = string.Join(" | ", createAdmin.Errors.Select(e => e.Description));
            return BadRequest(res);
        }
    }
}
