using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class UserMangeUser
    {
        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public string EmployeeId { get; set; }
        public ApplicationUser Employee {  get; set; }
    }
}
