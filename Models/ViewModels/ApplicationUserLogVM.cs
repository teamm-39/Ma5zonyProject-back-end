using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class ApplicationUserLogVM
    {
        public int ApplicationUserLogId { get; set; }
        public string UserName { get; set; }
        public DateTime DateTime { get; set; }
        public string OldName { get; set; }
        public int OldAge { get; set; }
        public string OldUserName { get; set; }
        public string OldEmail { get; set; }
        public string OldPhoneNumber { get; set; }
        public string? OldAddress { get; set; }
        public string? OldImgUrl { get; set; }
        public string NewName { get; set; }
        public int NewAge { get; set; }
        public string NewUserName { get; set; }
        public string NewEmail { get; set; }
        public string NewPhoneNumber { get; set; }
        public string? NewAddress { get; set; }
        public string? NewImgUrl { get; set; }
        public string Message { get; set; }
        public int LookupOperationTypeId { get; set; }
    }
}
