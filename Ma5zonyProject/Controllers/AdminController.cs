using DataAccess.IRepos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using Models.ViewModels;
using System.Security.Claims;
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
        private readonly ApplicationUserLogIRepo _log;
        public AdminController(ApplicationUserIRepo users,
                        UserManager<ApplicationUser> userManager,
                        RoleManager<IdentityRole> roleManager, ApplicationUserLogIRepo log)
        {
            _users = users;
            _userManager = userManager;
            _roleManager = roleManager;
            _log = log;
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted == true)
            {
                res.Meesage = "يرجى تسجيل الدخول اولا";
                return Unauthorized(res);
            }
            if (pageSize < 1 || pageNumber < 1)
            {
                res.Meesage = "رقم الصفحة وعدد العناصر يجب أن يكونا أكبر من 0";
                return BadRequest(res);
            }
            var adminUser = await _userManager.GetUsersInRoleAsync(StaticData.admin);
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
                                            expression: e => e.Id != userId && adminIds.Contains(e.Id) && e.IsDeleted == false
                                        );
            res.IsSuccess = true;
            res.Data = adminUsers.Data?.Select(user => new AdminsDTO
            {
                Id = user.Id,
                Address = user.Address,
                Age = user.Age,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Name = user.Name,
                UserName = user.UserName,
                ImgUrl = user.ImgUrl,
            }).ToList();
            res.Total = adminUsers.Total;
            return Ok(res);
        }
        [HttpPost("create")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> CreateAdmin([FromForm] UserDTO user, IFormFile? img)
        {
            var res = new Result<UserDTO>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userLogedIn = await _userManager.FindByIdAsync(userId);
            if (userLogedIn == null || userLogedIn.IsDeleted == true)
            {
                res.Meesage = "يرجى تسجيل الدخول اولا";
                return Unauthorized(res);
            }
            if (!ModelState.IsValid)
            {
                res.Meesage = "يوجد خطأ فى البيانات التى تم ارسالها";
                return BadRequest(res);
            }
            if (!_roleManager.Roles.Any())
            {
                await _roleManager.CreateAsync(new(roleName: StaticData.user));
                await _roleManager.CreateAsync(new(roleName: StaticData.admin));
            }
            if (await _roleManager.Roles.FirstOrDefaultAsync(r => r.Name == StaticData.admin) == null)
            {
                await _roleManager.CreateAsync(new IdentityRole(StaticData.admin));
            }
            if (await _roleManager.Roles.FirstOrDefaultAsync(r => r.Name == StaticData.user) == null)
            {
                await _roleManager.CreateAsync(new IdentityRole(StaticData.user));
            }
            var existingEmail = await _userManager.FindByEmailAsync(user.Email);
            if (existingEmail != null)
            {
                res.Meesage = "هذا البريد الإلكتروني مستخدم بالفعل.";
                return BadRequest(res);
            }
            var existingUserName = await _userManager.FindByNameAsync(user.UserName);
            if (existingUserName != null)
            {
                res.Meesage = "اسم المستخدم مستخدم بالفعل.";
                return BadRequest(res);
            }
            if (user.Age <= 0)
            {
                res.Meesage = "يرجى ادخال العمر بشكل صحيح";
                return BadRequest(res);
            }
            if (img != null)
            {
                // السماح فقط بامتدادات الصور
                var allowedExtensions = new HashSet<string> { ".jpg", ".jpeg", ".png", ".bmp" };
                var imgExtension = Path.GetExtension(img.FileName).ToLower();

                if (!allowedExtensions.Contains(imgExtension))
                {
                    res.Meesage = "يُسمح فقط بتحميل الصور بامتدادات: JPG, JPEG, PNG, BMP.";
                    return BadRequest(res);
                }
            }
            string? imgName = img != null ? Guid.NewGuid().ToString() + Path.GetExtension(img.FileName) : null;
            ApplicationUser createdUser = new()
            {
                Name = user.Name,
                Email = user.Email,
                Age = user.Age,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                ImgUrl = imgName,
                UserName = user.UserName,
            };
            var createUser = await _userManager.CreateAsync(createdUser, user.Password);
            if (createUser.Succeeded)
            {
                if (img != null)
                {
                    var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/profilePicture");
                    await FileHelper.SaveFileAsync(img, folder, imgName);
                }
                res.IsSuccess = true;
                await _userManager.AddToRoleAsync(createdUser, StaticData.admin);
                _log.CreateOperationLog(null, createdUser, StaticData.AddOperationType, userId, StaticData.admin);
                return Ok(res);
            }
            res.Meesage = string.Join(" | ", createUser.Errors.Select(e => e.Description));
            return BadRequest(res);
        }
        [HttpGet("details/{id}")]
        public async Task<ActionResult> GetOne(string id)
        {
            var res = new Result<UserDTO>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userLogedIn = await _userManager.FindByIdAsync(userId);
            if (userLogedIn == null || userLogedIn.IsDeleted == true)
            {
                res.Meesage = "يرجى تسجيل الدخول اولا";
                return Unauthorized(res);
            }
            if (string.IsNullOrEmpty(id))
            {
                res.Meesage = "لا يمكن ترك المعرف فارغا";
                return BadRequest(res);
            }
            var getUser = await _userManager.FindByIdAsync(id);
            if (getUser == null || !await _userManager.IsInRoleAsync(getUser, StaticData.admin) || getUser.IsDeleted == true)
            {
                res.Meesage = "لم يتم العثور على هذا المالك";
                return BadRequest(res);
            }
            UserDTO user = new UserDTO
            {
                Id = id,
                Address = getUser.Address,
                Age = getUser.Age,
                Name = getUser.Name,
                UserName = getUser.UserName,
                Email = getUser.Email,
                PhoneNumber = getUser.PhoneNumber,
                ImgUrl = getUser.ImgUrl
            };
            res.IsSuccess = true;
            res.Data = user;
            return Ok(res);
        }
        [HttpPut("edit")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> Edit([FromForm] AdminDTO newAdmin, IFormFile? img)
        {
            var res = new Result<AdminDTO>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userLogedIn = await _userManager.FindByIdAsync(userId);
            if (userLogedIn == null || userLogedIn.IsDeleted == true)
            {
                res.Meesage = "يرجى تسجيل الدخول اولا";
                return Unauthorized(res);
            }
            if (!ModelState.IsValid)
            {
                res.Meesage = "يوجد خطأ فى البيانات التى تم اراسلها";
                return BadRequest(res);
            }
            if (string.IsNullOrEmpty(newAdmin.Id))
            {
                res.Meesage = "لم يتم ارسال المعرف الشخصى";
                return BadRequest(res);
            }
            var admin = await _userManager.FindByIdAsync(newAdmin.Id);
            if (admin == null || !await _userManager.IsInRoleAsync(admin, StaticData.admin) || admin.IsDeleted == true)
            {
                res.Meesage = "لم يتم العثور على هذا المالك";
                return BadRequest(res);
            }
            if (admin.Email != newAdmin.Email)
            {
                var existingEmail = await _userManager.FindByEmailAsync(newAdmin.Email);
                if (existingEmail != null && existingEmail.Id != admin.Id)
                {
                    res.Meesage = "هذا البريد الإلكتروني مستخدم بالفعل.";
                    return BadRequest(res);
                }
            }
            if (admin.UserName != newAdmin.UserName)
            {
                var existingUserName = await _userManager.FindByNameAsync(newAdmin.UserName);
                if (existingUserName != null && existingUserName.Id != admin.Id)
                {
                    res.Meesage = "اسم المستخدم تم استخدامه";
                    return BadRequest(res);
                }
            }
            var oldAdmin = new ApplicationUser { Address = admin.Address, Email = admin.Email, Age = admin.Age, ImgUrl = admin.ImgUrl, Name = admin.Name, UserName = admin.UserName, PhoneNumber = admin.PhoneNumber };
            admin.Email = newAdmin.Email;
            admin.UserName = newAdmin.UserName;
            admin.Address = newAdmin.Address;
            admin.Age = newAdmin.Age;
            admin.PhoneNumber = newAdmin.PhoneNumber;
            admin.Name = newAdmin.Name;

            if (!string.IsNullOrEmpty(newAdmin.Password))
            {
                var removeOldPassword = await _userManager.RemovePasswordAsync(admin);
                var addNewPassWord = await _userManager.AddPasswordAsync(admin, newAdmin.Password);
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
                if (!string.IsNullOrEmpty(admin.ImgUrl))
                {
                    FileHelper.DeleteFile(folderPath, admin.ImgUrl);
                }
                admin.ImgUrl = Guid.NewGuid().ToString() + imgEx;
                await FileHelper.SaveFileAsync(img, folderPath, admin.ImgUrl);
            }
            var updateAdmin = await _userManager.UpdateAsync(admin);
            if (!updateAdmin.Succeeded)
            {
                res.Meesage = "حدث خطأ اثناء تحديث الادمن";
                return BadRequest(res);
            }
            res.IsSuccess = true;
            _log.CreateOperationLog(oldAdmin, admin, StaticData.EditOperationType, userId, StaticData.admin);
            return Ok(res);
        }
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> Delete([FromRoute] string id)
        {
            var res = new Result<AdminDTO>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userLogedIn = await _userManager.FindByIdAsync(userId);
            if (userLogedIn == null || userLogedIn.IsDeleted == true)
            {
                res.Meesage = "يرجى تسجيل الدخول اولا";
                return Unauthorized(res);
            }
            if (string.IsNullOrEmpty(id))
            {
                res.Meesage = "لم يتم ارسال المعرف الشخصى";
                return BadRequest(res);
            }
            var getAdmin = await _userManager.FindByIdAsync(id);
            if (getAdmin == null || !await _userManager.IsInRoleAsync(getAdmin, StaticData.admin) || getAdmin.IsDeleted == true)
            {
                res.Meesage = "لم يتم العثور على المالك";
                return BadRequest(res);
            }
            if (userId == getAdmin.Id)
            {
                res.Meesage = "انت هذا المالك لا يمكن حذف بياناتك بنفسك";
                return BadRequest(res);
            }
            getAdmin.IsDeleted = true;
            var deleteAdmin = await _userManager.UpdateAsync(getAdmin);
            if (!deleteAdmin.Succeeded)
            {
                res.Meesage = "حدث خطأ اثناء الحذف";
                return BadRequest(res);
            }
            res.IsSuccess = true;
            _log.CreateOperationLog(getAdmin, null, StaticData.DeleteOperationType, userId, StaticData.admin);
            return Ok(res);
        }
    }
}
