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
                ImgUrl = user.ImgUrl
            }).ToList();
            res.Total = adminUsers.Total;
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
                    res.Meesage = "يُسمح فقط بتحميل الصور بامتدادات: JPG, JPEG, PNG, BMP.";
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
                    var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/profilePicture");
                    await FileHelper.SaveFileAsync(img, folder, imgName);
                }
                res.IsSuccess = true;
                await _userManager.AddToRoleAsync(user, StaticData.admin);
                return Ok(res);
            }
            res.Meesage = string.Join(" | ", createAdmin.Errors.Select(e => e.Description));
            return BadRequest(res);
        }
        [HttpGet("details/{id}")]
        public async Task<ActionResult> GetOne(string id)
        {
            var res = new Result<AdminDTO>();
            if (string.IsNullOrEmpty(id))
            {
                res.Meesage = "لا يمكن ترك المعرف فارغا";
                return BadRequest(res);
            }
            var getAdmin = await _userManager.FindByIdAsync(id);
            if (getAdmin == null || !await _userManager.IsInRoleAsync(getAdmin, StaticData.admin))
            {
                res.Meesage = "لم يتم العثور على هذا الادمن";
                return BadRequest(res);
            }
            AdminDTO admin = new AdminDTO
            {
                Id = id,
                Address = getAdmin.Address,
                Age = getAdmin.Age,
                Name = getAdmin.Name,
                UserName = getAdmin.UserName,
                Email = getAdmin.Email,
                PhoneNumber = getAdmin.PhoneNumber,
                ImgUrl = getAdmin.ImgUrl
            };
            res.IsSuccess = true;
            res.Data = admin;
            return Ok(res);
        }
        [HttpPut("edit")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> Edit([FromForm] AdminDTO newAdmin, IFormFile? img)
        {
            var res = new Result<AdminDTO>();
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
            if (admin == null)
            {
                res.Meesage = "لم يتم العثور على هذا الادمن";
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
            if (img != null && img.Length > 0)
            {
                var allowedExtensions = new HashSet<string> { ".jpg", ".jpeg", ".png", ".bmp" };
                var imgExtension = Path.GetExtension(img.FileName).ToLower();

                if (!allowedExtensions.Contains(imgExtension))
                {
                    res.Meesage = "يُسمح فقط بتحميل الصور بامتدادات: JPG, JPEG, PNG, BMP.";
                    return BadRequest(res);
                }
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/profilePicture");
                var imgEx = Path.GetExtension(img.FileName);
                if (!string.IsNullOrEmpty(admin.ImgUrl))
                {
                    var oldPath = Path.Combine(folderPath, admin.ImgUrl);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
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
            res.IsSuccess=true;
            return Ok(res);
        }
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> Delete([FromRoute ]string id)
        {
            var res=new Result<AdminDTO>();
            if (string.IsNullOrEmpty(id))
            {
                res.Meesage = "لم يتم ارسال المعرف الشخصى";
                return BadRequest(res);
            }
            var getAdmin=await _userManager.FindByIdAsync(id);
            if(getAdmin == null)
            {
                res.Meesage = "لم يتم العثور على الادمن";
                return BadRequest(res);
            }
            var deleteAdmin=await _userManager.DeleteAsync(getAdmin);
            if (!deleteAdmin.Succeeded)
            {
                res.Meesage = "حدث خطأ اثناء الحذف";
                return BadRequest(res);
            }
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/profilePicture");
            if (!string.IsNullOrEmpty(getAdmin.ImgUrl))
            {
                FileHelper.DeleteFile(folderPath, getAdmin.ImgUrl);
            }
            res.IsSuccess = true;
            return Ok(res);
        }
    }
}
