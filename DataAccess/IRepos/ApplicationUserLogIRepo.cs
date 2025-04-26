using Models.Models;
using Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepos
{
    public interface ApplicationUserLogIRepo:IBaseRepo<ApplicationUserLog>
    {
        public void CreateOperationLog(ApplicationUser? oldUser, ApplicationUser? newUser, int operationType, string userId, string roleName);
        public List<ApplicationUserLogVM> GetAllWithoutPagination(
Expression<Func<ApplicationUserLog, object>>[]? includes = null,
Expression<Func<ApplicationUserLog, bool>>? expression = null,
Dictionary<string, object>? filters = null);
    }
}
