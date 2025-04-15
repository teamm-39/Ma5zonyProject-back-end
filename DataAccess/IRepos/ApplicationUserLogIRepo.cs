using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepos
{
    public interface ApplicationUserLogIRepo:IBaseRepo<ApplicationUserLog>
    {
        public void CreateOperationLog(ApplicationUser? oldUser, ApplicationUser? newUser, int operationType, string userId, string roleName);

    }
}
