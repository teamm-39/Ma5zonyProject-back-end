using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public int Age { get; set; }


        public int? StoreId { get; set; }
        public Store? Store { get; set; }
        // المستخدمين اللي يديرهم هذا المستخدم
        public ICollection<UserMangeUser> ManagedEmployees { get; set; }

        // المديرين اللي يدير هذا المستخدم
        public ICollection<UserMangeUser> Managers { get; set; }
        public ICollection<UserMangeStore> UserMangeStores { get; set; }
        public ICollection<UserMangeProduct> UserMangeProducts { get; set; }
        public ICollection<Export> Exports { get; set; }
        public ICollection<Import> Imports { get; set; }
    }
}
