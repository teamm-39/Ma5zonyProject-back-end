using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Utility;
using Microsoft.AspNetCore.Identity;
using Models.Models;
using Models.ViewModels;
using DataAccess.IRepos;
using Microsoft.EntityFrameworkCore;
namespace Ma5zonyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationUserIRepo _users;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(ApplicationUserIRepo users,
                        UserManager<ApplicationUser> userManager,
                        RoleManager<IdentityRole> roleManager)
        {
            _users = users;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllUsers(
            int pageSize = 5,
            int pageNumber = 1
            , string? name = null
            , string? userName = null
            , int? age = null
            , string? phone = null
            , string? address = null)
        {
            var res = new Result<List<UsersDTO>>(isSuccess: false, message: "",
                                              pageNumber: pageNumber, pageSize: pageSize, data: []);
            if (pageSize < 1 || pageNumber < 1)
            {
                res.Meesage = "رقم الصفحة وعدد العناصر يجب أن يكونا أكبر";
                return BadRequest(res);
            }
            var user = await _userManager.GetUsersInRoleAsync("user");
            var usersIds = user.Select(u => u.Id).ToList();
            //fiteration
            var filter = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(name)) filter.Add("Name", name);
            if (!string.IsNullOrEmpty(userName)) filter.Add("UserName", userName);
            if (age.HasValue) filter.Add("Age", age);
            if (!string.IsNullOrEmpty(phone)) filter.Add("PhoneNumber", phone);
            if (!string.IsNullOrEmpty(address)) filter.Add("Address", address);
            //data
            var users = _users.GetAll(
                                            pageNumber: pageNumber,
                                            pageSize: pageSize,
                                            filters: filter,
                                            expression: e => usersIds.Contains(e.Id) && e.IsDeleted == false
                                        );
            res.IsSuccess = true;

            res.Data = users.Data?.Select(user => new UsersDTO
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
            res.Total = users.Total;
            return Ok(res);
        }
        [HttpPost("create")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> CreateUser([FromForm] UserDTO userDTO, IFormFile? img)
        {
            var res = new Result<UserDTO>();
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
            var existingEmail = await _userManager.FindByEmailAsync(userDTO.Email);
            if (existingEmail != null)
            {
                res.Meesage = "هذا البريد الإلكتروني مستخدم بالفعل.";
                return BadRequest(res);
            }
            var existingUserName = await _userManager.FindByNameAsync(userDTO.UserName);
            if (existingUserName != null)
            {
                res.Meesage = "اسم المستخدم مستخدم بالفعل.";
                return BadRequest(res);
            }
            if (userDTO.Age <= 0)
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
            ApplicationUser user = new()
            {
                Name = userDTO.Name,
                Email = userDTO.Email,
                Age = userDTO.Age,
                PhoneNumber = userDTO.PhoneNumber,
                Address = userDTO.Address,
                ImgUrl = imgName,
                UserName = userDTO.UserName,
            };
            var createUser = await _userManager.CreateAsync(user, userDTO.Password);
            if (createUser.Succeeded)
            {
                if (img != null)
                {
                    var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/profilePicture");
                    await FileHelper.SaveFileAsync(img, folder, imgName);
                }
                res.IsSuccess = true;
                await _userManager.AddToRoleAsync(user, StaticData.user);
                return Ok(res);
            }
            res.Meesage = string.Join(" | ", createUser.Errors.Select(e => e.Description));
            return BadRequest(res);
        }
        [HttpGet("details/{id}")]
        public async Task<ActionResult> GetOne(string id)
        {
            var res = new Result<UserDTO>();
            if (string.IsNullOrEmpty(id))
            {
                res.Meesage = "لا يمكن ترك المعرف فارغا";
                return BadRequest(res);
            }
            var getUser = await _userManager.FindByIdAsync(id);
            if (getUser == null || !await _userManager.IsInRoleAsync(getUser, StaticData.user) || getUser.IsDeleted == true)
            {
                res.Meesage = "لم يتم العثور على هذا المتسخدم";
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
        public async Task<ActionResult> Edit([FromForm] UserDTO newUser, IFormFile? img)
        {
            var res = new Result<UserDTO>();
            if (!ModelState.IsValid)
            {
                res.Meesage = "يوجد خطأ فى البيانات التى تم اراسلها";
                return BadRequest(res);
            }
            if (string.IsNullOrEmpty(newUser.Id))
            {
                res.Meesage = "لم يتم ارسال المعرف الشخصى";
                return BadRequest(res);
            }
            var user = await _userManager.FindByIdAsync(newUser.Id);
            if (user == null || !await _userManager.IsInRoleAsync(user, StaticData.user) || user.IsDeleted == true)
            {
                res.Meesage = "لم يتم العثور على هذا المستخدم";
                return BadRequest(res);
            }
            if (user.Email != newUser.Email)
            {
                var existingEmail = await _userManager.FindByEmailAsync(newUser.Email);
                if (existingEmail != null && existingEmail.Id != user.Id)
                {
                    res.Meesage = "هذا البريد الإلكتروني مستخدم بالفعل.";
                    return BadRequest(res);
                }
            }
            if (user.UserName != newUser.UserName)
            {
                var existingUserName = await _userManager.FindByNameAsync(newUser.UserName);
                if (existingUserName != null && existingUserName.Id != user.Id)
                {
                    res.Meesage = "اسم المستخدم تم استخدامه";
                    return BadRequest(res);
                }
            }
            user.Email = newUser.Email;
            user.UserName = newUser.UserName;
            user.Address = newUser.Address;
            user.Age = newUser.Age;
            user.PhoneNumber = newUser.PhoneNumber;
            user.Name = newUser.Name;

            if (!string.IsNullOrEmpty(newUser.Password))
            {
                var removeOldPassword = await _userManager.RemovePasswordAsync(user);
                var addNewPassWord = await _userManager.AddPasswordAsync(user, newUser.Password);
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
            var updateUser = await _userManager.UpdateAsync(user);
            if (!updateUser.Succeeded)
            {
                res.Meesage = "حدث خطأ اثناء تحديث المستخدم";
                return BadRequest(res);
            }
            res.IsSuccess = true;
            return Ok(res);
        }
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> Delete([FromRoute] string id)
        {
            var res = new Result<UserDTO>();
            if (string.IsNullOrEmpty(id))
            {
                res.Meesage = "لم يتم ارسال المعرف الشخصى";
                return BadRequest(res);
            }
            var getUser = await _userManager.FindByIdAsync(id);
            if (getUser == null || !await _userManager.IsInRoleAsync(getUser, StaticData.user) || getUser.IsDeleted == true)
            {
                res.Meesage = "لم يتم العثور على المستخدم";
                return BadRequest(res);
            }
            getUser.IsDeleted = true;
            var deleteUser = await _userManager.UpdateAsync(getUser);
            if (!deleteUser.Succeeded)
            {
                res.Meesage = "حدث خطأ اثناء الحذف";
                return BadRequest(res);
            }
            res.IsSuccess = true;
            return Ok(res);
        }
    }
}
