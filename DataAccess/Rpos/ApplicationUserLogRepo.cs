using DataAccess.Data;
using DataAccess.IRepos;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DataAccess.Rpos
{
    public class ApplicationUserLogRepo : BaseRepo<ApplicationUserLog>, ApplicationUserLogIRepo
    {
        ApplicationDbContext _context;
        public ApplicationUserLogRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public void CreateOperationLog(ApplicationUser? oldUser, ApplicationUser? newUser, int operationType, string userId, string roleName)
        {
            var log = new ApplicationUserLog();
            if (operationType == StaticData.AddOperationType)
            {
                log = new ApplicationUserLog
                {
                    ApplicationUserId = userId,
                    RoleName = roleName,
                    OldAddress = "-",
                    OldAge = 0,
                    OldEmail = "-",
                    OldImgUrl = "-",
                    OldName = "-",
                    OldPhoneNumber = "-",
                    OldUserName = "-",
                    NewAddress = newUser.Address,
                    NewAge = newUser.Age,
                    NewEmail = newUser.Email,
                    NewImgUrl = newUser.ImgUrl,
                    NewName = newUser.Name,
                    NewPhoneNumber = newUser.PhoneNumber,
                    NewUserName = newUser.UserName,
                    Message = "تم اضافة المستخدم"
                };
            }
            else if (operationType == StaticData.EditOperationType)
            {
                log = new ApplicationUserLog
                {
                    ApplicationUserId = userId,
                    RoleName = roleName,
                    OldAddress = oldUser.Address,
                    OldAge = oldUser.Age,
                    OldEmail = oldUser.Email,
                    OldImgUrl = oldUser.ImgUrl,
                    OldName = oldUser.Name,
                    OldPhoneNumber = oldUser.PhoneNumber,
                    OldUserName = oldUser.UserName,
                    NewAddress = newUser.Address,
                    NewAge = newUser.Age,
                    NewEmail = newUser.Email,
                    NewImgUrl = newUser.ImgUrl,
                    NewName = newUser.Name,
                    NewPhoneNumber = newUser.PhoneNumber,
                    NewUserName = newUser.UserName,
                    Message = "تم تعديل المستخدم"
                };
            }
            else
            {
                log = new ApplicationUserLog
                {
                    ApplicationUserId = userId,
                    RoleName = roleName,
                    OldAddress = oldUser.Address,
                    OldAge = oldUser.Age,
                    OldEmail = oldUser.Email,
                    OldImgUrl = oldUser.ImgUrl,
                    OldName = oldUser.Name,
                    OldPhoneNumber = oldUser.PhoneNumber,
                    OldUserName = oldUser.UserName,
                    NewAddress = "-",
                    NewAge = 0,
                    NewEmail = "-",
                    NewImgUrl = "-",
                    NewName = "-",
                    NewPhoneNumber = "-",
                    NewUserName = "-",
                    Message = "تم حذف المستخدم"
                };
            }
            log.LookupOperationTypeId = operationType;
            log.DateTime= DateTime.Now;
            _context.ApplicationUserLogs.Add(log);
            _context.SaveChanges();
        }
    }
}
